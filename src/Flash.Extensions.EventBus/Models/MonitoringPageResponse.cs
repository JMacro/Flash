using System.Collections.Generic;

namespace Flash.Extensions.EventBus
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class MonitoringPageResponse<TData> : MonitoringPageResponse
    {

        /// <summary>
        /// 分页数据
        /// </summary>
        public virtual List<TData> List { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MonitoringPageResponse
    {
        /// <summary>
        /// 页码
        /// </summary>
        public virtual int PageIndex { get; set; }
        /// <summary>
        /// 页容量
        /// </summary>
        public virtual int PageSize { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public virtual int PageCount { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public virtual int TotalCount { get; set; }
        /// <summary>
        /// 过滤数
        /// </summary>
        public virtual int FilteredCount { get; set; }
    }
}
