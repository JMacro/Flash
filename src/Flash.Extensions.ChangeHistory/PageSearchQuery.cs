using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.ChangeHistory
{
    /// <summary>
    /// 变更历史分页查询条件
    /// </summary>
    public class PageSearchQuery : IPageQuery
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public PageOrderBy OrderBy { get; set; }
        /// <summary>
        /// 实体对象Id
        /// </summary>
        public string EntityId { get; set; }
        /// <summary>
        /// 创建时间(开始)
        /// </summary>
        public DateTime? BeginCreateTime { get; set; }
        /// <summary>
        /// 创建时间(结束)
        /// </summary>
        public DateTime? EndCreateTime { get; set; }
    }
}
