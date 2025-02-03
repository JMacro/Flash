namespace Flash.Extensions.EventBus.RabbitMQ
{
    public sealed class RabbitMqQueueMessageDto : QueueMessageDto
    {
        /// <summary>
        /// 正常队列名称
        /// </summary>
        public override string NormalQueueName { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public override string Message { get; set; }
        /// <summary>
        /// 消息id
        /// </summary>
        public override string MessageId { get; set; }
    }
}
