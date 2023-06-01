using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.EventBus
{
    public interface IEventBus
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="messages">消息内容</param>
        /// <param name="confirm">是否采用发布确认机制</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> PublishAsync(List<MessageCarrier> messages, bool confirm = true, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="confirm">是否采用发布确认机制</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> PublishAsync(MessageCarrier message, bool confirm = true, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 注册订阅处理程序
        /// </summary>
        /// <typeparam name="TMessage">消息类型</typeparam>
        /// <typeparam name="TProcessMessageHandler">消息处理程序</typeparam>
        /// <param name="queueName">队列名称</param>
        /// <param name="routeKey">路由名称</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IEventBus Register<TMessage, TProcessMessageHandler>(string queueName = "", string routeKey = "", CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class where TProcessMessageHandler : IProcessMessageHandler<TMessage>;

        /// <summary>
        /// 注册订阅处理程序
        /// </summary>
        /// <typeparam name="TMessage">消息类型</typeparam>
        /// <typeparam name="TProcessMessageHandler">消息处理程序（此处存在消息重复被消费问题，客户端需做好幂等操作）</typeparam>
        /// <param name="queueName">队列名称</param>
        /// <param name="routeKey">路由名称</param>
        /// <param name="ackHandler">应答处理程序（未传入则使用系统默认）</param>
        /// <param name="nackHandler">未应答处理程序（未传入则使用系统默认）</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IEventBus Register<TMessage, TProcessMessageHandler>(string queueName, string routeKey, Action<(MessageResponse Message, IMessageAckHandler Handler)> ackHandler, Func<(MessageResponse Message, IMessageAckHandler Handler, Exception Exception), Task<bool>> nackHandler, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class where TProcessMessageHandler : IProcessMessageHandler<TMessage>;

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="ackHandler">Ack应答处理程序</param>
        /// <param name="nackHandler">NAck应答处理程序</param>
        /// <returns></returns>
        IEventBus Subscriber(Action<MessageResponse[]> ackHandler, Func<(MessageResponse[] Messages, Exception Exception), Task<bool>> nackHandler);
    }
}
