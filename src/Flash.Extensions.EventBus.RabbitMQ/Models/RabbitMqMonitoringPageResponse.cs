using Newtonsoft.Json;
using System.Collections.Generic;

namespace Flash.Extensions.EventBus.RabbitMQ
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class RabbitMqMonitoringPageResponse<TData> : MonitoringPageResponse<TData>
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("page")]
        public override int PageIndex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("page_size")]
        public override int PageSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("page_count")]
        public override int PageCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("total_count")]
        public override int TotalCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("filtered_count")]
        public override int FilteredCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("items")]
        public override List<TData> List { get; set; }
    }
}
