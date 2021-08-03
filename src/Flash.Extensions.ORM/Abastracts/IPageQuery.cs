namespace Flash.Extensions.ORM
{
    public enum PageOrderBy
    {
        DESC,
        ASC
    }

    public interface IPageQuery
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
        /// 排序类型
        /// </summary>
        PageOrderBy OrderBy { get; set; }
    }

    public class PageQuery : IPageQuery
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
        /// 排序类型
        /// </summary>
        public PageOrderBy OrderBy { get; set; } = PageOrderBy.DESC;
    }

    public static class PageQueryExtension
    {
        /// <summary>
        /// 创建分页查询参数
        /// </summary>
        /// <typeparam name="TSearchPara"></typeparam>
        /// <param name="pageQuery"></param>
        /// <param name="searchPara"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IPageQuery CreateNew(int pageIndex, int pageSize)
        {
            var result = new PageQuery();
            result.PageIndex = pageIndex;
            result.PageSize = pageSize;
            return result;
        }
    }
}
