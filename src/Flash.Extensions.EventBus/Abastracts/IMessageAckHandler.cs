using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extensions.EventBus
{
    /// <summary>
    /// 消息应答处理接口
    /// </summary>
    public interface IMessageAckHandler
    {
        /// <summary>
        /// 应答处理程序
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        Task AckHandle(MessageResponse response);

        /// <summary>
        /// 未应答处理程序
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        Task NAckHandle(MessageResponse response, Exception ex);
    }
}
