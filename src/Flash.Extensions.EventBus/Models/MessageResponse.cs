using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.EventBus
{
    public class MessageResponse
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
        /// 事件请求头
        /// </summary>
        public IDictionary<string, object> Headers { get; set; }
        /// <summary>
        /// 事件数据
        /// </summary>
        public dynamic Body { get; set; }
        /// <summary>
        /// 事件数据
        /// </summary>
        public string BodySource { get; set; }
        /// <summary>
        /// 队列名称
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// 路由名称
        /// </summary>
        public string RouteKey { get; set; }
        /// <summary>
        /// 发送次数
        /// </summary>
        public int TimesSent { get; set; }
    }
}
