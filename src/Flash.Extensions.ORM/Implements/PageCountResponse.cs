using System.Collections.Generic;

namespace Flash.Extensions.ORM
{
    public class PageCountResponse<TData> : BasePageResponse<TData>
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public int Total { get; set; }


        public PageCountResponse(List<TData> list, int pageIndex, int pageSize, int total) : base(list, pageIndex, pageSize)
        {
            this.Total = total;
        }
    }
}
