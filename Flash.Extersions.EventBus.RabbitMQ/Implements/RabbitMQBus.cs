using Flash.Extersions.OpenTracting;
using Flash.LoadBalancer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Timeout;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extersions.EventBus.RabbitMQ
{
    /// <summary>
    /// RabbitMQ总线
    /// </summary>
    public class RabbitMQBus : IEventBus
    {
        /// <summary>
        /// 消息参数体
        /// </summary>
        internal struct MessageRequest
        {
            /// <summary>
            /// 载具编号
            /// </summary>
            public string CarrierId { get; set; }
            /// <summary>
            /// 消息Id
            /// </summary>
            public string MessageId { get; set; }
            /// <summary>
            /// 消息体
            /// </summary>
            public string Body { get; set; }
            /// <summary>
            /// 路由Key
            /// </summary>
            public string RouteKey { get; set; }
            /// <summary>
            /// 消息队列相关参数信息
            /// </summary>
            public IDictionary<string, object> Headers { get; set; }
        }

        /// <summary>
        /// DI容器
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// 日志收集器
        /// </summary>
        private readonly ILogger<IEventBus> _logger;

        /// <summary>
        /// Qos策略（默认为1，同一时刻服务器最大接收1个消息，如未确认则不会收到下一个消息）
        /// </summary>
        private readonly ushort _prefetchCount = 1;
        /// <summary>
        /// 交换机名称
        /// </summary>
        private readonly string _exchange = "amq.topic";
        /// <summary>
        /// 交换机类型
        /// </summary>
        private readonly string _exchangeType = "topic";

        #region 消费者参数
        /// <summary>
        /// 消费者均衡机
        /// </summary>
        private readonly ILoadBalancer<IRabbitMQPersistentConnection> _receiveLoadBlancer;
        /// <summary>
        /// 消费者最大并行数
        /// </summary>
        private readonly int _reveiverMaxDegreeOfParallelism;
        /// <summary>
        /// 事件总线消费者策略
        /// </summary>
        private readonly IAsyncPolicy _receiverPolicy = null;
        #endregion

        #region 生产者参数
        /// <summary>
        /// 生产者均衡机
        /// </summary>
        private readonly ILoadBalancer<IRabbitMQPersistentConnection> _senderLoadBlancer;
        /// <summary>
        /// 事件总线生产者策略
        /// </summary>
        private readonly IAsyncPolicy _senderRetryPolicy = null;
        /// <summary>
        /// 生产者确认超时时间（单位毫秒）
        /// </summary>
        private readonly int _senderConfirmTimeoutMillseconds;
        #endregion

        /// <summary>
        /// 应答处理程序
        /// </summary>
        private Action<MessageResponse[]> _ackHandler = null;
        /// <summary>
        /// 未应答处理程序
        /// </summary>
        private Func<(MessageResponse[] Messages, Exception Exception), Task<bool>> _nackHandler = null;
        private ITracerFactory _tracerFactory = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="senderLoadBlancer"></param>
        /// <param name="receiveLoadBlancer"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <param name="reveiverMaxDegreeOfParallelism">消费者最大并行数</param>
        /// <param name="reveiverRetryCount">消费者处理程序异常重试次数</param>
        /// <param name="receiverHandlerTimeoutMillseconds">消费者处理程序超时时间（单位毫秒）</param>
        /// <param name="senderRetryCount">生产者策略重试次数</param>
        /// <param name="senderConfirmTimeoutMillseconds">生产者发送消息确认超时时间（单位毫秒）</param>
        /// <param name="prefetchCount">Qos策略（默认为1，同一时刻服务器最大接收1个消息，如未确认则不会收到下一个消息）</param>
        /// <param name="exchange">交换机名称</param>
        /// <param name="exchangeType">交换机类型</param>
        public RabbitMQBus(
            ILoadBalancer<IRabbitMQPersistentConnection> senderLoadBlancer,
            ILoadBalancer<IRabbitMQPersistentConnection> receiveLoadBlancer,
            IServiceProvider serviceProvider,
            ILogger<IEventBus> logger,
            int reveiverMaxDegreeOfParallelism = 10,
            int reveiverRetryCount = 0,
            int receiverHandlerTimeoutMillseconds = 5000,
            int senderRetryCount = 3,
            int senderConfirmTimeoutMillseconds = 500,
            ushort prefetchCount = 1,
            string exchange = "amp.topic",
            string exchangeType = "topic")
        {
            this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this._reveiverMaxDegreeOfParallelism = reveiverMaxDegreeOfParallelism;
            this._prefetchCount = prefetchCount;
            this._exchange = exchange;
            this._exchangeType = exchangeType;

            this._tracerFactory = _serviceProvider.GetService(typeof(ITracerFactory)) as ITracerFactory;

            #region 生产者策略
            this._senderLoadBlancer = senderLoadBlancer;
            this._senderConfirmTimeoutMillseconds = senderConfirmTimeoutMillseconds;
            this._senderRetryPolicy = Policy.NoOpAsync();
            this._senderRetryPolicy = _senderRetryPolicy.WrapAsync(Policy.Handle<BrokerUnreachableException>()
                    .Or<SocketException>()
                    .Or<IOException>()
                    .Or<AlreadyClosedException>()
                    .WaitAndRetryAsync(senderRetryCount, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogError(ex.ToString());
                    }));
            #endregion

            #region 消费者策略
            this._receiveLoadBlancer = receiveLoadBlancer;
            this._receiverPolicy = Policy.NoOpAsync();//创建一个空的Policy
            if (reveiverRetryCount > 0)
            {
                //设置重试策略
                this._receiverPolicy = _receiverPolicy.WrapAsync(Policy.Handle<Exception>()
                    .RetryAsync(reveiverRetryCount, (ex, time) =>
                    {
                        _logger.LogError(ex, ex.ToString());
                    }));
            }

            if (receiverHandlerTimeoutMillseconds > 0)
            {
                // 设置超时
                this._receiverPolicy = _receiverPolicy.WrapAsync(Policy.TimeoutAsync(TimeSpan.FromSeconds(receiverHandlerTimeoutMillseconds), TimeoutStrategy.Pessimistic, (context, timespan, task) =>
                {
                    return Task.FromResult(true);
                }));
            }
            #endregion
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="messages">消息内容</param>
        /// <param name="confirm">是否采用发布确认机制</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<bool> PublishAsync(List<MessageCarrier> messages, bool confirm = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await EnqueueConfirm(Convert4MessageRequest(messages), true, cancellationToken);
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="confirm">是否采用发布确认机制</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<bool> PublishAsync(MessageCarrier message, bool confirm = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await EnqueueConfirm(Convert4MessageRequest(new List<MessageCarrier> { message }), true, cancellationToken);
        }

        /// <summary>
        /// 注册订阅处理程序
        /// </summary>
        /// <typeparam name="TMessage">消息类型</typeparam>
        /// <typeparam name="TProcessMessageHandler">消息处理程序（此处存在消息重复被消费问题，客户端需做好幂等操作）</typeparam>
        /// <param name="queueName">队列名称</param>
        /// <param name="routeKey">路由名称</param>
        /// <returns></returns>
        public IEventBus Register<TMessage, TProcessMessageHandler>(string queueName = "", string routeKey = "")
            where TMessage : class
            where TProcessMessageHandler : IProcessMessageHandler<TMessage>
        {
            var connection = _receiveLoadBlancer.Resolve();
            if (!connection.IsConnected) connection.TryConnect();

            for (int i = 0; i < _reveiverMaxDegreeOfParallelism; i++)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        var _channel = connection.GetModel();
                        var _queueName = string.IsNullOrEmpty(queueName) ? typeof(TProcessMessageHandler).FullName : queueName;
                        var _routeKey = string.IsNullOrEmpty(routeKey) ? typeof(TMessage).FullName : routeKey;
                        var EventAction = _serviceProvider.GetService(typeof(TProcessMessageHandler)) as IProcessMessageHandler<TMessage>;
                        if (EventAction == null)
                        {
                            EventAction = System.Activator.CreateInstance(typeof(TProcessMessageHandler)) as IProcessMessageHandler<TMessage>;
                        }

                        _channel.ExchangeDeclare(_exchange, _exchangeType, true, false, null);
                        //在MQ上定义一个持久化队列，如果名称相同不会重复创建
                        _channel.QueueDeclare(_queueName, true, false, false, null);
                        //绑定交换器和队列
                        _channel.QueueBind(_queueName, _exchange, _routeKey);
                        //绑定交换器和队列
                        _channel.QueueBind(_queueName, _exchange, _queueName);
                        //输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
                        _channel.BasicQos(0, _prefetchCount, false);
                        //在队列上定义一个消费者a
                        EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
                        consumer.Received += async (ch, ea) =>
                        {
                            using (var tracer = _tracerFactory.CreateTracer("AMQP Received"))
                            {
                                #region AMQP Received
                                try
                                {
                                    #region Ensure IsConnected
                                    if (!connection.IsConnected)
                                    {
                                        connection.TryConnect();
                                    }
                                    #endregion

                                    var MessageId = string.IsNullOrEmpty(ea.BasicProperties.MessageId) ? Guid.NewGuid().ToString("N") : ea.BasicProperties.MessageId;

                                    string carrierId = "";
                                    if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.ContainsKey("x-carrier-id"))
                                    {
                                        carrierId = Encoding.UTF8.GetString((ea.BasicProperties.Headers["x-carrier-id"] as byte[]));
                                        tracer.SetTag("x-carrier-id", carrierId);
                                    }

                                    tracer.SetTag("x-queue-name", _queueName);
                                    tracer.SetTag("x-message-id", MessageId);
                                    tracer.SetTag("x-router-key", _routeKey);
                                    tracer.SetTag("x-exchange-name", _exchange);
                                    tracer.SetTag("x-exchange-type", _exchangeType);

                                    var messageResponse = new MessageResponse()
                                    {
                                        MessageId = MessageId,
                                        Headers = ea.BasicProperties.Headers ?? new Dictionary<string, object>(),
                                        Body = default(TMessage),
                                        QueueName = _queueName,
                                        RouteKey = _routeKey,
                                        BodySource = Encoding.UTF8.GetString(ea.Body),
                                        CarrierId = carrierId
                                    };

                                    try
                                    {
                                        messageResponse.Body = JsonConvert.DeserializeObject<TMessage>(messageResponse.BodySource);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, ex.Message);
                                    }

                                    if (!messageResponse.Headers.ContainsKey("x-exchange"))
                                    {
                                        messageResponse.Headers.Add("x-exchange", _exchange);
                                    }

                                    if (!messageResponse.Headers.ContainsKey("x-exchange-type"))
                                    {
                                        messageResponse.Headers.Add("x-exchange-type", _exchangeType);
                                    }

                                    #region AMQP ExecuteAsync
                                    using (var tracerExecute = _tracerFactory.CreateTracer("AMQP Execute"))
                                    {
                                        var handlerException = default(Exception);

                                        var handlerOK = false;
                                        tracerExecute.SetTag("x-queue-name", _queueName);
                                        tracerExecute.SetTag("x-message-id", MessageId);
                                        tracerExecute.SetTag("x-router-key", _routeKey);
                                        tracerExecute.SetTag("x-exchange-name", _exchange);
                                        tracerExecute.SetTag("x-exchange-type", _exchangeType);

                                        try
                                        {
                                            handlerOK = await _receiverPolicy.ExecuteAsync(async (cancellationToken) =>
                                            {
                                                //开始执行客户端处理程序，并获得执行结果，此处存在消息重复被消费问题。客户端需做好幂等操作
                                                return await EventAction.Handle(messageResponse.Body, (Dictionary<string, object>)messageResponse.Headers, cancellationToken);
                                            }, CancellationToken.None);

                                            if (handlerOK)
                                            {
                                                if (_ackHandler != null)
                                                {
                                                    _ackHandler(new MessageResponse[] { messageResponse });
                                                }

                                                //确认消息
                                                _channel.BasicAck(ea.DeliveryTag, false);

                                            }
                                            else
                                            {
                                                tracerExecute.SetError();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            tracerExecute.SetError();
                                            handlerException = ex;
                                            _logger.LogError(ex, ex.Message);
                                        }
                                        finally
                                        {
                                            if (!handlerOK)
                                            {
                                                //重新入队，默认：是
                                                var requeue = true;

                                                try
                                                {
                                                    //执行回调，等待业务层的处理结果
                                                    if (_nackHandler != null)
                                                    {
                                                        requeue = await _nackHandler((new MessageResponse[] { messageResponse }, handlerException));
                                                    }
                                                }
                                                catch (Exception innterEx)
                                                {
                                                    _logger.LogError(innterEx, innterEx.Message);
                                                }

                                                //确认消息
                                                _channel.BasicReject(ea.DeliveryTag, requeue);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    tracer.SetError();
                                    _logger.LogError(ex.Message, ex);
                                }
                                #endregion
                            }
                        };
                        consumer.Unregistered += (ch, ea) =>
                        {
                            _logger.LogDebug($"MQ:{_queueName} Consumer_Unregistered");
                        };
                        consumer.Registered += (ch, ea) =>
                        {
                            _logger.LogDebug($"MQ:{_queueName} Consumer_Registered");
                        };
                        consumer.Shutdown += (ch, ea) =>
                        {
                            _logger.LogDebug($"MQ:{_queueName} Consumer_Shutdown.{ea.ReplyText}");
                        };
                        consumer.ConsumerCancelled += (object sender, ConsumerEventArgs e) =>
                        {
                            _logger.LogDebug($"MQ:{_queueName} ConsumerCancelled");
                        };

                        //消费队列，并设置应答模式为程序主动应答
                        _channel.BasicConsume(_queueName, false, consumer);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }

                });
            }
            return this;
        }

        /// <summary>
        /// 排队入队确认
        /// </summary>
        /// <param name="messages">消息载体</param>
        /// <returns></returns>
        private async Task<bool> EnqueueConfirm(List<MessageRequest> messages, bool confirm, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connection = _senderLoadBlancer.Resolve();
            try
            {
                if (!connection.IsConnected) connection.TryConnect();

                var _channel = connection.GetModel();

                // 提交走批量通道
                var _batchPublish = _channel.CreateBasicPublishBatch();

                for (int i = 0; i < messages.Count; i++)
                {
                    await this._senderRetryPolicy.ExecuteAsync((e) =>
                    {

                        var routeKey = messages[i].RouteKey;
                        byte[] bytes = Encoding.UTF8.GetBytes(messages[i].Body);

                        //设置消息持久化
                        IBasicProperties properties = _channel.CreateBasicProperties();
                        properties.DeliveryMode = 2;
                        properties.MessageId = messages[i].MessageId;
                        properties.Headers = new Dictionary<string, Object>();
                        properties.Headers["x-carrier-id"] = messages[i].CarrierId;


                        foreach (var key in messages[i].Headers.Keys)
                        {
                            if (!properties.Headers.ContainsKey(key))
                            {
                                properties.Headers.Add(key, messages[i].Headers[key]);
                            }
                        }

                        if (messages[i].Headers.ContainsKey("x-first-death-queue"))
                        {
                            //延时队列或者直接写死信的情况
                            var newQueue = messages[i].Headers["x-first-death-queue"].ToString();

                            //创建一个队列                         
                            _channel.QueueDeclare(
                                           queue: newQueue,
                                           durable: true,
                                           exclusive: false,
                                           autoDelete: false,
                                           arguments: messages[i].Headers);

                            _batchPublish.Add(
                                    exchange: "",
                                    mandatory: true,
                                    routingKey: newQueue,
                                    properties: properties,
                                    body: bytes);
                        }
                        else
                        {
                            //发送到正常队列
                            _batchPublish.Add(
                                       exchange: _exchange,
                                       mandatory: true,
                                       routingKey: routeKey,
                                       properties: properties,
                                       body: bytes);
                        }

                        return Task.FromResult(true);
                    }, cancellationToken);
                }

                //批量提交
                _batchPublish.Publish();

                if (confirm)
                {
                    return _channel.WaitForConfirms(TimeSpan.FromMilliseconds(500));
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// 转换为MessageRequest消息载体对象
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        private List<MessageRequest> Convert4MessageRequest(List<MessageCarrier> messages)
        {
            var messageCarrier = messages.Select(item => new MessageRequest
            {
                Body = item.Content,
                Headers = item.Headers ?? new Dictionary<string, object>(),
                MessageId = Guid.NewGuid().ToString("N"),
                RouteKey = item.RouteKey,
                CarrierId = item.CarrierId
            }).ToList();

            messageCarrier.ForEach(item =>
            {
                if (!item.Headers.ContainsKey("x-ts"))
                {
                    item.Headers.Add("x-ts", DateTime.UtcNow.ToTimestamp());
                }
            });
            return messageCarrier;
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="ackHandler"></param>
        /// <param name="nackHandler"></param>
        /// <returns></returns>
        public IEventBus Subscriber(Action<MessageResponse[]> ackHandler, Func<(MessageResponse[] Messages, Exception Exception), Task<bool>> nackHandler)
        {
            _ackHandler = ackHandler;
            _nackHandler = nackHandler;
            return this;
        }
    }
}
