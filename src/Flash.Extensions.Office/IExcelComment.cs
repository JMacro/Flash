namespace Flash.Extensions.Office
{
    /// <summary>
    /// Excel批注描述
    /// </summary>
    public interface IExcelComment
    {
        /// <summary>
        /// 默认显示模式(显示\隐藏)
        /// </summary>
        bool DefaultVisible { get; set; }
        /// <summary>
        /// 批注内容
        /// </summary>
        string Content { get; set; }
        /// <summary>
        /// 批注作者
        /// </summary>
        string Author { get; set; }
    }
}
