namespace Flash.Extensions.Office
{
    /// <summary>
    /// Excel批注描述
    /// </summary>
    public class ExcelComment : IExcelComment
    {
        /// <summary>
        /// 默认显示模式(显示\隐藏)
        /// </summary>
        public bool DefaultVisible { get; set; } = false;
        /// <summary>
        /// 批注内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 批注作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 创建Excel批注描述
        /// </summary>
        /// <param name="content">批注内容</param>
        /// <returns></returns>
        public static ExcelComment Create(string content)
        {
            return Create(content, "System", false);
        }

        /// <summary>
        /// 创建Excel批注描述
        /// </summary>
        /// <param name="content">批注内容</param>
        /// <param name="isVisible">默认显示模式(显示\隐藏)</param>
        /// <returns></returns>
        public static ExcelComment Create(string content, bool isVisible)
        {
            return Create(content, "System", isVisible);
        }

        /// <summary>
        /// 创建Excel批注描述
        /// </summary>
        /// <param name="content">批注内容</param>
        /// <param name="author">批注作者</param>
        /// <returns></returns>
        public static ExcelComment Create(string content, string author)
        {
            return Create(content, author, false);
        }

        /// <summary>
        /// 创建Excel批注描述
        /// </summary>
        /// <param name="content">批注内容</param>
        /// <param name="author">批注作者</param>
        /// <param name="isVisible">默认显示模式(显示\隐藏)</param>
        /// <returns></returns>
        public static ExcelComment Create(string content, string author, bool isVisible)
        {
            return new ExcelComment
            {
                Author = author,
                Content = content,
                DefaultVisible = isVisible
            };
        }
    }
}
