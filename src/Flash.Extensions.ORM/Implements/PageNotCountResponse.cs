using System.Collections.Generic;
using System.Linq;

namespace Flash.Extensions.ORM
{
    public class PageNotCountResponse<TData> : BasePageResponse<TData>
    {
        public override bool HasData => this.List != null && this.List.Any();

        public PageNotCountResponse(List<TData> list, int pageIndex, int pageSize) : base(list, pageIndex, pageSize)
        {
        }
    }
}
