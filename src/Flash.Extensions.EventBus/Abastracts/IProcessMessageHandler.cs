﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.EventBus
{
    /// <summary>
    /// 消息处理接口
    /// <para>如需对消息应答个性化处理请实现<see cref="IMessageAckHandler"/>接口</para>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IProcessMessageHandler<in TMessage> where TMessage : class
    {
        /// <summary>
        /// 消息处理程序
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="headers">消息队列相关参数信息</param>
        /// <param name="cancellationToken">传播应取消操作的通知</param>
        /// <returns></returns>
        Task<bool> Handle(TMessage message, Dictionary<string, object> headers, CancellationToken cancellationToken);
    }
}
