using Flash.LoadBalancer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
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

namespace Flash.Extersions.RabbitMQ
{
    public class RabbitMQBus : IBus
    {
        /// <summary>
        /// 消息参数体
        /// </summary>
        public struct MessageRequest
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



        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<IBus> _logger;
        private readonly ILoadBalancer<IRabbitMQPersistentConnection> _publishLoadBlancer;
        private readonly ILoadBalancer<IRabbitMQPersistentConnection> _subscribeLoadBlancer;
        private static List<IModel> _subscribeChannels = new List<IModel>();

        private readonly int _reveiverMaxDegreeOfParallelism;
        private readonly ushort _prefetchCount = 1;
        private readonly string _exchange = "amq.topic";
        private readonly string _exchangeType = "topic";
        private readonly RetryPolicy _eventBusPublishRetryPolicy = null;
        private readonly IAsyncPolicy _eventBusReceiverPolicy = null;
        private Action<MessageResponse[]> _ackHandler = null;
        private Func<(MessageResponse[] Messages, Exception Exception), Task<bool>> _nackHandler = null;

        public RabbitMQBus(
            ILoadBalancer<IRabbitMQPersistentConnection> publishLoadBlancer,
            ILoadBalancer<IRabbitMQPersistentConnection> subscribeLoadBlancer,
            IServiceProvider serviceProvider,
            ILogger<IBus> logger,
            int reveiverMaxDegreeOfParallelism = 10,
            int subscribeRetryCount = 0,
            int receiverHandlerTimeoutMillseconds = 0,
            int publishRetryCount = 3,
            ushort prefetchCount = 1,
            string exchange = "amp.topic",
            string exchangeType = "topic")
        {
            this._publishLoadBlancer = publishLoadBlancer;
            this._subscribeLoadBlancer = subscribeLoadBlancer;
            this._serviceProvider = serviceProvider;
            this._logger = logger;
            this._reveiverMaxDegreeOfParallelism = reveiverMaxDegreeOfParallelism;
            this._prefetchCount = prefetchCount;
            this._exchange = exchange;
            this._exchangeType = exchangeType;

            #region 生产者策略
            this._eventBusPublishRetryPolicy = RetryPolicy.Handle<BrokerUnreachableException>()
                    .Or<SocketException>()
                    .Or<IOException>()
                    .Or<AlreadyClosedException>()
                    .WaitAndRetry(publishRetryCount, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogError(ex.ToString());
                    });
            #endregion

            #region 消费者策略
            _eventBusReceiverPolicy = Policy.NoOpAsync();//创建一个空的Policy

            if (subscribeRetryCount > 0)
            {
                //设置重试策略
                _eventBusReceiverPolicy = _eventBusReceiverPolicy.WrapAsync(Policy.Handle<Exception>()
                    .RetryAsync(subscribeRetryCount, (ex, time) =>
                    {
                        _logger.LogError(ex, ex.ToString());
                    }));
            }

            if (receiverHandlerTimeoutMillseconds > 0)
            {
                // 设置超时
                _eventBusReceiverPolicy = _eventBusReceiverPolicy.WrapAsync(Policy.TimeoutAsync(TimeSpan.FromSeconds(receiverHandlerTimeoutMillseconds), TimeoutStrategy.Pessimistic, (context, timespan, task) =>
                {
                    return Task.FromResult(true);
                }));
            }
            #endregion
        }

        public async Task<bool> PublishAsync(List<MessageCarrier> messages)
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
            return await EnqueueConfirm(messageCarrier);
        }

        public IBus Register<TMessage, TProcessMessageHandler>(string queueName = "", string routeKey = "")
            where TMessage : class
            where TProcessMessageHandler : IProcessMessageHandler<TMessage>
        {
            var connection = _subscribeLoadBlancer.Resolve();
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

                            #region AMQP Received
                            try
                            {
                                #region Ensure IsConnected
                                if (!connection.IsConnected)
                                {
                                    connection.TryConnect();
                                }
                                #endregion

                                string carrierId = "";
                                if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.ContainsKey("x-carrier-id"))
                                {
                                    carrierId = Encoding.UTF8.GetString((ea.BasicProperties.Headers["x-carrier-id"] as byte[]));
                                }

                                var messageResponse = new MessageResponse()
                                {
                                    MessageId = string.IsNullOrEmpty(ea.BasicProperties.MessageId) ? Guid.NewGuid().ToString("N") : ea.BasicProperties.MessageId,
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

                                try
                                {
                                    var handlerOK = await _eventBusReceiverPolicy.ExecuteAsync(async (cancellationToken) =>
                                    {
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
                                        //重新入队，默认：是
                                        var requeue = true;

                                        try
                                        {
                                            //执行回调，等待业务层确认是否重新入队
                                            if (_nackHandler != null)
                                            {
                                                requeue = await _nackHandler((new MessageResponse[] { messageResponse }, null));
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
                                catch (Exception ex)
                                {

                                    //重新入队，默认：是
                                    var requeue = true;

                                    try
                                    {
                                        //执行回调，等待业务层的处理结果
                                        if (_nackHandler != null)
                                        {
                                            requeue = await _nackHandler((new MessageResponse[] { messageResponse }, ex));
                                        }
                                    }
                                    catch (Exception innterEx)
                                    {
                                        _logger.LogError(innterEx, innterEx.Message);
                                    }

                                    //确认消息
                                    _channel.BasicReject(ea.DeliveryTag, requeue);
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex.Message, ex);
                            }
                            #endregion
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

                        _subscribeChannels.Add(_channel);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                });
            }
            return this;
        }

        private async Task<bool> EnqueueConfirm(List<MessageRequest> messages)
        {
            var connection = _publishLoadBlancer.Resolve();
            try
            {
                if (!connection.IsConnected) connection.TryConnect();

                var _channel = connection.GetModel();

                // 提交走批量通道
                var _batchPublish = _channel.CreateBasicPublishBatch();

                return await System.Threading.Tasks.Task<bool>.Run(() =>
                {
                    messages.ForEach(item =>
                    {
                        var routeKey = item.RouteKey;
                        byte[] bytes = Encoding.UTF8.GetBytes(item.Body);

                        //设置消息持久化
                        IBasicProperties properties = _channel.CreateBasicProperties();
                        properties.DeliveryMode = 2;
                        properties.MessageId = item.MessageId;
                        properties.Headers = new Dictionary<string, Object>();
                        properties.Headers["x-carrier-id"] = item.CarrierId;


                        foreach (var key in item.Headers.Keys)
                        {
                            if (!properties.Headers.ContainsKey(key))
                            {
                                properties.Headers.Add(key, item.Headers[key]);
                            }
                        }

                        if (item.Headers.ContainsKey("x-first-death-queue"))
                        {
                            //延时队列或者直接写死信的情况
                            var newQueue = item.Headers["x-first-death-queue"].ToString();

                            //创建一个队列                         
                            _channel.QueueDeclare(
                                           queue: newQueue,
                                           durable: true,
                                           exclusive: false,
                                           autoDelete: false,
                                           arguments: item.Headers);

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


                    });

                    //批量提交
                    _batchPublish.Publish();

                    return _channel.WaitForConfirms(TimeSpan.FromMilliseconds(500));
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;

            }
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="ackHandler"></param>
        /// <param name="nackHandler"></param>
        /// <returns></returns>
        public IBus Subscribe(Action<MessageResponse[]> ackHandler, Func<(MessageResponse[] Messages, Exception Exception), Task<bool>> nackHandler)
        {
            _ackHandler = ackHandler;
            _nackHandler = nackHandler;
            return this;
        }
    }
}
