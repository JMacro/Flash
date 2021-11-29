using Newtonsoft.Json;

namespace Flash.Extensions.EventBus.RabbitMQ
{
    public sealed class RabbitMqQueueWithAllEnqueuedDto
    {
        [JsonProperty("vhost")]
        public string VHost { get; set; }
        [JsonProperty("name")]
        public string QueueName { get; set; }
        [JsonProperty("messages")]
        public long MessageCount { get; set; }
        [JsonProperty("node")]
        public string NodeName { get; set; }
    }
}
