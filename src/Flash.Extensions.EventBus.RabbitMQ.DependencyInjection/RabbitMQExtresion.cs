using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Flash.Extensions.EventBus.RabbitMQ
{
    /// <summary>
    /// MQ操作扩展
    /// </summary>
    public static class RabbitMQExtresion
    {
        /// <summary>
        /// 注册订阅处理程序(使用重试策略)
        /// <para>处理程序需实现<see cref="IMessageAckHandler"/>接口，如不实现则消息不进行重试策略处理</para>
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TProcessMessageHandler"></typeparam>
        /// <param name="event"></param>
        /// <param name="queueName">队列名称</param>
        /// <param name="routeKey">路由名称</param>
        /// <param name="retryAttempt">等待时间(秒)</param>
        /// <param name="maxRetries">重试次数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static IEventBus RegisterWaitAndRetry<TMessage, TProcessMessageHandler>(this IEventBus @event, string queueName, string routeKey, int retryAttempt = 5, int maxRetries = 3, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class where TProcessMessageHandler : IProcessMessageHandler<TMessage>
        {
            @event.Register<TMessage, TProcessMessageHandler>(queueName, routeKey, (obj) =>
            {
                if (obj.Handler != null)
                {
                    obj.Handler.AckHandle(obj.Message);
                }
            }, async (obj) =>
            {
                if (obj.Handler != null)
                {
                    await obj.Handler.NAckHandle(obj.Message, obj.Exception);
                }
                var result = !(await @event.PublishAsync(obj.Message.WaitAndRetry(a => retryAttempt, maxRetries)));
                return result;
            }, cancellationToken);
            return @event;
        }

        /// <summary>
        /// 注册订阅处理程序(使用重试策略)
        /// <para>处理程序需实现<see cref="IMessageAckHandler"/>接口，如不实现则消息不进行重试策略处理</para>
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TProcessMessageHandler"></typeparam>
        /// <param name="event"></param>
        /// <param name="retryAttempt">等待时间(秒)</param>
        /// <param name="maxRetries">重试次数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static IEventBus RegisterWaitAndRetry<TMessage, TProcessMessageHandler>(this IEventBus @event, int retryAttempt = 5, int maxRetries = 3, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class where TProcessMessageHandler : IProcessMessageHandler<TMessage>
        {
            @event.RegisterWaitAndRetry<TMessage, TProcessMessageHandler>(typeof(TProcessMessageHandler).FullName, typeof(TMessage).FullName, retryAttempt, maxRetries, cancellationToken);
            return @event;
        }

        /// <summary>
        /// 注册订阅处理程序(使用延迟策略)
        /// <para>处理程序需实现<see cref="IMessageAckHandler"/>接口，如不实现则消息不进行重试策略处理</para>
        /// <para>需安装rabbitmq_delayed_message_exchange插件，插件安装请查看https://github.com/rabbitmq/rabbitmq-delayed-message-exchange</para>
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TProcessMessageHandler"></typeparam>
        /// <param name="event"></param>
        /// <param name="queueName">队列名称</param>
        /// <param name="routeKey">路由名称</param>
        /// <param name="retryAttempt">等待时间(秒)</param>
        /// <param name="maxRetries">重试次数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static IEventBus RegisterDelay<TMessage, TProcessMessageHandler>(this IEventBus @event, string queueName, string routeKey, int retryAttempt = 5, int maxRetries = 3, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class where TProcessMessageHandler : IProcessMessageHandler<TMessage>
        {
            @event.Register<TMessage, TProcessMessageHandler>(queueName, routeKey, "x-delayed-message", (obj) =>
            {
                if (obj.Handler != null)
                {
                    obj.Handler.AckHandle(obj.Message);
                }
            }, async (obj) =>
            {
                if (obj.Handler != null)
                {
                    await obj.Handler.NAckHandle(obj.Message, obj.Exception);
                }
                var result = !(await @event.PublishAsync(obj.Message.WaitAndRetry(a => retryAttempt, maxRetries)));
                return result;
            }, new Dictionary<string, object>() { { "x-delayed-type", "direct" } });
            return @event;
        }

        /// <summary>
        /// 注册订阅处理程序(使用延迟策略)
        /// <para>处理程序需实现<see cref="IMessageAckHandler"/>接口，如不实现则消息不进行重试策略处理</para>
        /// <para>需安装rabbitmq_delayed_message_exchange插件，插件安装请查看https://github.com/rabbitmq/rabbitmq-delayed-message-exchange</para>
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TProcessMessageHandler"></typeparam>
        /// <param name="event"></param>
        /// <param name="retryAttempt">等待时间(秒)</param>
        /// <param name="maxRetries">重试次数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static IEventBus RegisterDelay<TMessage, TProcessMessageHandler>(this IEventBus @event, int retryAttempt = 5, int maxRetries = 3, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class where TProcessMessageHandler : IProcessMessageHandler<TMessage>
        {
            @event.RegisterDelay<TMessage, TProcessMessageHandler>(typeof(TProcessMessageHandler).FullName, typeof(TMessage).FullName, retryAttempt, maxRetries, cancellationToken);
            return @event;
        }
    }
}
