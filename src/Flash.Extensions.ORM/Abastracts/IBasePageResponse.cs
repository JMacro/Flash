using System.Collections.Generic;

namespace Flash.Extensions.ORM
{
    public interface IBasePageResponse<TData>
    {
        /// <summary>
        /// 页码
        /// </summary>
        int PageIndex { get; set; }
        /// <summary>
        /// 页容量
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// 分页数据
        /// </summary>
        List<TData> List { get; set; }
        /// <summary>
        /// 是否有数据
        /// </summary>
        bool HasData { get; }
    }
}
