using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Flash.Extensions.EventBus
{
    /// <summary>
    /// 消息载具
    /// </summary>
    public class MessageCarrier
    {
        private MessageCarrier()
        {
            this.CreationTime = DateTime.Now;
            this.State = MessageState.NotPublished;
            this.TimesSent = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeKey">路由名称</param>
        /// <param name="message">消息主体</param>
        public MessageCarrier(string routeKey, object message) : this()
        {
            this.Headers = new Dictionary<string, object>();
            this.RouteKey = string.IsNullOrEmpty(routeKey) ? message.GetType().FullName : routeKey;
            this.Content = JsonConvert.SerializeObject(message);
            this.CarrierId = Guid.NewGuid().ToString("N");
            this.MessageId = Guid.NewGuid().ToString("N");
        }


        public IDictionary<string, object> Headers { get; set; }
        /// <summary>
        /// 载具编号
        /// </summary>
        public string CarrierId { get; protected set; }
        /// <summary>
        /// 消息编号
        /// </summary>
        public string MessageId { get; set; }
        /// <summary>
        /// 路由Key
        /// </summary>
        public string RouteKey { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public MessageState State { get; protected set; }
        /// <summary>
        /// 发送次数
        /// </summary>
        public int TimesSent { get; protected set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; protected set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        public static MessageCarrier Clone(MessageResponse response)
        {
            return new MessageCarrier()
            {
                CarrierId = response.CarrierId,
                MessageId = response.MessageId,
                Headers = response.Headers,
                Content = response.BodySource,
                RouteKey = response.QueueName,
                State = MessageState.NotPublished,
                TimesSent = response.TimesSent
            };
        }
    }
}
