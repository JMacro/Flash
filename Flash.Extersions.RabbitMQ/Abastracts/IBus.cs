using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extersions.RabbitMQ
{
    /// <summary>
    /// 消息总线接口
    /// </summary>
    public interface IBus
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="Events">消息内容</param>
        /// <returns></returns>
        Task<bool> PublishAsync(List<MessageCarrier> messages);

        /// <summary>
        /// 注册订阅处理程序
        /// </summary>
        /// <typeparam name="TMessage">消息类型</typeparam>
        /// <typeparam name="TProcessMessageHandler">消息处理程序</typeparam>
        /// <param name="queueName">队列名称</param>
        /// <param name="routeKey">路由名称</param>
        /// <returns></returns>
        IBus Register<TMessage, TProcessMessageHandler>(string queueName = "", string routeKey = "") where TMessage : class where TProcessMessageHandler : IProcessMessageHandler<TMessage>;

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="ackHandler">Ack应答处理程序</param>
        /// <param name="nackHandler">NAck应答处理程序</param>
        /// <returns></returns>
        IBus Subscribe(Action<MessageResponse[]> ackHandler, Func<(MessageResponse[] Messages, Exception Exception), Task<bool>> nackHandler);
    }
}
