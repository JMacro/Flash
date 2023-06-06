using Flash.Extensions.EventBus;
using Flash.Extensions.EventBus.RabbitMQ;
using Flash.LoadBalancer;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public class RabbitMQOption
    {
        #region Endpoint
        /// <summary>
        /// 服务器地址(默认:localhost)
        /// </summary>
        internal string HostName { get; set; } = "locahost";
        /// <summary>
        /// 端口（默认：5672）
        /// </summary>
        internal int Port { get; set; } = 5672;
        /// <summary>
        /// 仪表盘管理协议
        /// </summary>
        internal string DashboardProtocol = "http";
        /// <summary>
        /// 仪表盘管理端口（默认：15672）
        /// </summary>
        internal int DashboardPort { get; set; } = 15672;
        #endregion

        #region Auth

        /// <summary>
        /// 账号(默认:guest)
        /// </summary>
        internal string UserName { get; set; } = "guest";

        /// <summary>
        /// 密码(默认:guest)
        /// </summary>
        internal string Password { get; set; } = "guest";
        #endregion

        #region Exchange
        /// <summary>
        /// 虚拟主机(默认：/)
        /// </summary>
        internal string VirtualHost { get; set; } = "/";


        /// <summary>
        /// 交换机名称(默认：amq.topic)
        /// </summary>
        internal string Exchange { get; set; } = "amq.topic";

        /// <summary>
        /// 交换机类型（默认：topic）
        /// </summary>
        internal string ExchangeType { get; set; } = "topic";
        #endregion

        #region Sender
        /// <summary>
        /// 重试次数(默认：3)
        /// </summary>
        internal int SenderAcquireRetryAttempts { get; set; } = 3;

        internal int SenderMaxConnections { get; set; } = 10;

        internal LoadBalancerType SenderLoadBalancer { get; set; }

        #endregion

        #region Receiver

        /// <summary>
        /// 消费者连接数量
        /// </summary>
        internal int ReceiverMaxConnections { get; set; } = 2;
        /// <summary>
        /// 消费者负载均衡器
        /// </summary>
        internal LoadBalancerType ReceiverLoadBalancer { get; set; }

        /// <summary>
        /// 消费者最大重试次数
        /// </summary>
        internal int ReceiverAcquireRetryAttempts { get; set; } = 3;

        /// <summary>
        /// 消费处理超时时间
        /// </summary>
        internal int ReceiverHandlerTimeoutMillseconds { get; set; } = 1000 * 2;

        /// <summary>
        /// 消费单个连接最大Channel数量
        /// </summary>
        internal int ReveiverMaxDegreeOfParallelism { get; set; } = 10;

        /// <summary>
        /// 默认获取
        /// </summary>
        internal ushort PreFetch { get; set; } = 1;

        /// <summary>
        /// 幂等持续时间（默认:15秒）
        /// </summary>
        internal int IdempotencyDuration { get; set; } = 15;
        #endregion

        /// <summary>
        /// 队列前缀名称
        /// </summary>
        internal string QueuePrefixName { get; set; } = "";

        /// <summary>
        /// 终结点设置
        /// </summary>
        /// <param name="HostName">地址</param>
        /// <param name="Port">端口</param>
        public RabbitMQOption WithEndPoint(string HostName = "locahost", int Port = 5672)
        {
            this.HostName = HostName;
            this.Port = Port;
            return this;
        }

        /// <summary>
        /// 仪表盘管理设置
        /// </summary>
        /// <param name="dashboardProtocol">协议</param>
        /// <param name="dashboardPort">端口</param>
        /// <returns></returns>
        public RabbitMQOption WithDashboard(string dashboardProtocol = "http", int dashboardPort = 15672)
        {
            this.DashboardProtocol = dashboardProtocol;
            this.DashboardPort = dashboardPort;
            return this;
        }

        /// <summary>
        /// 设置认证信息
        /// </summary>
        /// <param name="UserName">账号</param>
        /// <param name="Password">密码</param>
        public RabbitMQOption WithAuth(string UserName, string Password)
        {
            this.UserName = UserName;
            this.Password = Password;
            return this;
        }

        /// <summary>
        /// 设置交换器信息
        /// </summary>
        /// <param name="VirtualHost">虚拟机</param>
        /// <param name="ExchangeType">交换器类型</param>
        /// <param name="Exchange">交换器名称</param>
        public RabbitMQOption WithExchange(string VirtualHost = "/", string ExchangeType = "topic", string Exchange = "amq.topic")
        {
            this.VirtualHost = VirtualHost;
            this.ExchangeType = ExchangeType;
            this.Exchange = Exchange;
            return this;
        }

        /// <summary>
        /// 设置连接池信息
        /// </summary>
        /// <param name="SenderMaxConnections">发送端最大连接数量</param>
        /// <param name="AcquireRetryAttempts">最大重试次数</param>
        /// <param name="LoadBalancer">负责均衡机类型</param>
        public RabbitMQOption WithSender(int SenderMaxConnections = 10, int AcquireRetryAttempts = 3, LoadBalancerType LoadBalancer = LoadBalancerType.Random)
        {
            this.SenderMaxConnections = SenderMaxConnections;
            this.SenderAcquireRetryAttempts = AcquireRetryAttempts;
            this.SenderLoadBalancer = LoadBalancer;
            return this;
        }

        /// <summary>
        /// 消费端设置
        /// </summary>
        /// <param name="ReceiverMaxConnections">消费最大连接数</param>
        /// <param name="ReveiverMaxDegreeOfParallelism">消费单个连接最大Channel数量</param>
        /// <param name="ReceiverAcquireRetryAttempts">最大重试次数</param>
        /// <param name="ReceiverHandlerTimeoutMillseconds">消费处理超时时间</param>
        /// <param name="IdempotencyDurationSeconds">幂等持续时间（秒）</param>
        /// <param name="PreFetch">预取数量</param>
        /// <param name="LoadBalancer">负责均衡机类型</param>
        public RabbitMQOption WithReceiver(
            int ReceiverMaxConnections = 2,
            int ReveiverMaxDegreeOfParallelism = 10,
            int ReceiverAcquireRetryAttempts = 0,
            int ReceiverHandlerTimeoutMillseconds = 0,
            LoadBalancerType LoadBalancer = LoadBalancerType.Random,
            int IdempotencyDurationSeconds = 15,
            ushort PreFetch = 1)
        {
            this.ReceiverMaxConnections = ReceiverMaxConnections;
            this.ReveiverMaxDegreeOfParallelism = ReveiverMaxDegreeOfParallelism;
            this.ReceiverAcquireRetryAttempts = ReceiverAcquireRetryAttempts;
            this.ReceiverHandlerTimeoutMillseconds = ReceiverHandlerTimeoutMillseconds;
            this.IdempotencyDuration = IdempotencyDurationSeconds;
            this.PreFetch = PreFetch;
            this.ReceiverLoadBalancer = LoadBalancer;
            return this;
        }

        /// <summary>
        /// 队列前缀名称设置
        /// </summary>
        /// <param name="queuePrefixName">队列前缀名称</param>
        /// <returns></returns>
        public RabbitMQOption WithQueuePrefixName(string queuePrefixName)
        {
            this.QueuePrefixName = queuePrefixName;
            return this;
        }
    }

    public static partial class DependencyInjectionExtersion
    {
        /// <summary>
        /// 使用RabbitMQ消息总线
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IEventBusHostBuilder UseRabbitMQ(this IEventBusHostBuilder hostBuilder, Action<RabbitMQOption> setup)
        {
            setup = setup ?? throw new ArgumentNullException(nameof(setup));

            var option = new RabbitMQOption();
            setup(option);
            hostBuilder.Services.AddSingleton(sp =>
            {
                return option;
            });
            hostBuilder.Services.AddSingleton<IMonitoringApi, RabbitMqMonitoringApi>();
            hostBuilder.Services.AddAutoMapper(config =>
            {
                config.CreateMap<RabbitMqMonitoringPageResponse<RabbitMqQueueWithAllEnqueuedDto>, MonitoringPageResponse<QueueWithAllEnqueuedDto>>();
                config.CreateMap<RabbitMqQueueWithAllEnqueuedDto, QueueWithAllEnqueuedDto>();
            });

            hostBuilder.Services.AddSingleton<IConnectionFactory>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<IConnectionFactory>>();

                var factory = new ConnectionFactory();
                factory.HostName = option.HostName;
                factory.Port = option.Port;
                factory.Password = option.Password;
                factory.UserName = option.UserName;
                factory.VirtualHost = option.VirtualHost;
                factory.AutomaticRecoveryEnabled = true;
                factory.TopologyRecoveryEnabled = true;
                factory.UseBackgroundThreadsForIO = true;
                return factory;
            });

            hostBuilder.Services.AddSingleton<ILoadBalancerFactory<IRabbitMQPersistentConnection>>(sp =>
            {
                return new DefaultLoadBalancerFactory<IRabbitMQPersistentConnection>();
            });

            hostBuilder.Services.AddSingleton<IEventBus, RabbitMQBus>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<IEventBus>>();
                var loggerConnection = sp.GetRequiredService<ILogger<IRabbitMQPersistentConnection>>();
                var rabbitMQPersisterConnectionLoadBalancerFactory = sp.GetRequiredService<ILoadBalancerFactory<IRabbitMQPersistentConnection>>();
                var connectionFactory = sp.GetRequiredService<IConnectionFactory>();
                var senderConnections = new List<IRabbitMQPersistentConnection>();
                var receiveConnections = new List<IRabbitMQPersistentConnection>();

                //消费端连接池
                for (int i = 0; i < option.ReceiverMaxConnections; i++)
                {
                    var connection = new RabbitMQPersistentConnection(connectionFactory, loggerConnection, option.ReceiverAcquireRetryAttempts);
                    connection.TryConnect();
                    //消费端的连接池
                    receiveConnections.Add(connection);
                }

                //发送端连接池
                for (int i = 0; i < option.SenderMaxConnections; i++)
                {
                    var connection = new RabbitMQPersistentConnection(connectionFactory, loggerConnection, option.SenderAcquireRetryAttempts);
                    connection.TryConnect();
                    senderConnections.Add(connection);
                }

                var receiveLoadBlancer = rabbitMQPersisterConnectionLoadBalancerFactory.Resolve(() => receiveConnections, option.ReceiverLoadBalancer);
                var senderLoadBlancer = rabbitMQPersisterConnectionLoadBalancerFactory.Resolve(() => senderConnections, option.SenderLoadBalancer);

                return new RabbitMQBus(
                    senderLoadBlancer: senderLoadBlancer,
                    receiveLoadBlancer: receiveLoadBlancer,
                    serviceProvider: sp,
                    logger: logger,
                    reveiverMaxDegreeOfParallelism: option.ReveiverMaxDegreeOfParallelism,
                    reveiverRetryCount: option.ReceiverAcquireRetryAttempts,
                    receiverHandlerTimeoutMillseconds: option.ReceiverHandlerTimeoutMillseconds,
                    senderRetryCount: option.SenderAcquireRetryAttempts,
                    senderConfirmTimeoutMillseconds: 500,
                    prefetchCount: option.PreFetch,
                    exchange: option.Exchange,
                    exchangeType: option.ExchangeType,
                    queuePrefixName: option.QueuePrefixName
                );
            });

            return hostBuilder;
        }
    }
}
