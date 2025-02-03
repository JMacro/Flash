namespace Flash.Extensions.EventBus
{
    public class QueueMessageDto
    {
        /// <summary>
        /// 正常队列名称
        /// </summary>
        public virtual string NormalQueueName { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public virtual string Message { get; set; }
        /// <summary>
        /// 消息id
        /// </summary>
        public virtual string MessageId { get; set; }
    }
}
