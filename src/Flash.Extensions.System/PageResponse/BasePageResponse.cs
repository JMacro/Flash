using System.Collections.Generic;

namespace Flash.Extensions
{
    public abstract class BasePageResponse<TData> : IBasePageResponse<TData>
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 页容量
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 分页数据
        /// </summary>
        public List<TData> List { get; set; }
        /// <summary>
        /// 是否有数据
        /// </summary>
        public virtual bool HasData { get; }

        public BasePageResponse(List<TData> list, int pageIndex, int pageSize)
        {
            this.List = list;
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
        }
    }
}
