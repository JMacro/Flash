namespace Flash.Extensions.EventBus
{
    public class QueueWithAllEnqueuedDto
    {
        /// <summary>
        /// 虚拟主机名称
        /// </summary>
        public virtual string VHost { get; set; }
        /// <summary>
        /// 队列名称
        /// </summary>
        public virtual string QueueName { get; set; }
        /// <summary>
        /// 消息数
        /// </summary>
        public virtual long MessageCount { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public virtual string NodeName { get; set; }
    }
}
