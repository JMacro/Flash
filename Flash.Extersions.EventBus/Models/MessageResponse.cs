using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extersions.EventBus
{
    public class MessageResponse
    {
        /// <summary>
        /// 载具编号
        /// </summary>
        public string CarrierId { get; set; }
        public string MessageId { get; set; }
        public IDictionary<string, object> Headers { get; set; }

        public dynamic Body { get; set; }

        public string QueueName { get; set; }

        public string RouteKey { get; set; }
        public string BodySource { get; set; }
        /// <summary>
        /// 发送次数
        /// </summary>
        public int TimesSent { get; set; }
    }
}
