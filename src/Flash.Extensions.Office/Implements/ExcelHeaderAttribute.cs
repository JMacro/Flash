using System;

namespace Flash.Extensions.Office
{
    /// <summary>
    /// Excel表头标注
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelHeaderAttribute : Attribute
    {
        /// <summary>
        /// Excel表头标注
        /// </summary>
        /// <param name="preEntityFieldName">上一个实体字段名称，用于表头排序（为空则表示第一列，如都为空则随机排序）</param>
        /// <param name="excelColumnName">Excel列名</param>
        /// <param name="dataFormat">数据格式</param>
        public ExcelHeaderAttribute(string preEntityFieldName, string excelColumnName, string dataFormat = "") : this(preEntityFieldName, excelColumnName, dataFormat, -1) { }

        /// <summary>
        /// Excel表头标注
        /// </summary>
        /// <param name="preEntityFieldName">上一个实体字段名称，用于表头排序（为空则表示第一列，如都为空则随机排序）</param>
        /// <param name="excelColumnName">Excel列名</param>
        /// <param name="width">表头宽度，默认-1表示根据表头列名动态宽度</param>
        public ExcelHeaderAttribute(string preEntityFieldName, string excelColumnName, short width) : this(preEntityFieldName, excelColumnName, "", width) { }

        /// <summary>
        /// Excel表头标注
        /// </summary>
        /// <param name="preEntityFieldName">上一个实体字段名称，用于表头排序（为空则表示第一列，如都为空则随机排序）</param>
        /// <param name="excelColumnName">Excel列名</param>
        /// <param name="width">表头宽度，默认-1表示根据表头列名动态宽度</param>
        /// <param name="excelCommentContent">批注内容</param>
        public ExcelHeaderAttribute(string preEntityFieldName, string excelColumnName, short width, string excelCommentContent) : this(preEntityFieldName, excelColumnName, "", width, excelCommentContent, null) { }

        /// <summary>
        /// Excel表头标注
        /// </summary>
        /// <param name="preEntityFieldName">上一个实体字段名称，用于表头排序（为空则表示第一列，如都为空则随机排序）</param>
        /// <param name="excelColumnName">Excel列名</param>
        /// <param name="width">表头宽度，默认-1表示根据表头列名动态宽度</param>
        /// <param name="excelCommentContent">批注内容</param>
        /// <param name="excelCommentAuthor">批注作者</param>
        public ExcelHeaderAttribute(string preEntityFieldName, string excelColumnName, short width, string excelCommentContent, string excelCommentAuthor) : this(preEntityFieldName, excelColumnName, "", width, excelCommentContent, excelCommentAuthor) { }

        /// <summary>
        /// Excel表头标注
        /// </summary>
        /// <param name="preEntityFieldName">上一个实体字段名称，用于表头排序（为空则表示第一列，如都为空则随机排序）</param>
        /// <param name="excelColumnName">Excel列名</param>
        /// <param name="dataFormat">数据格式</param>
        /// <param name="width">表头宽度，默认-1表示根据表头列名动态宽度</param>
        public ExcelHeaderAttribute(string preEntityFieldName, string excelColumnName, string dataFormat, short width) : this(preEntityFieldName, excelColumnName, dataFormat, width, null, null) { }

        /// <summary>
        /// Excel表头标注
        /// </summary>
        /// <param name="preEntityFieldName">上一个实体字段名称，用于表头排序（为空则表示第一列，如都为空则随机排序）</param>
        /// <param name="excelColumnName">Excel列名</param>
        /// <param name="dataFormat">数据格式</param>
        /// <param name="width">表头宽度，默认-1表示根据表头列名动态宽度</param>
        /// <param name="excelCommentContent">批注内容</param>
        /// <param name="excelCommentAuthor">批注作者</param>
        public ExcelHeaderAttribute(string preEntityFieldName, string excelColumnName, string dataFormat, short width, string excelCommentContent, string excelCommentAuthor)
        {
            this.PreEntityFieldName = preEntityFieldName;
            this.ExcelColumnName = excelColumnName;
            this.DataFormat = dataFormat;
            this.Width = width;
            this.CommentAuthor = excelCommentAuthor;
            this.CommentContent = excelCommentContent;
        }

        /// <summary>
        /// 上一个实体字段名称，用于表头排序（为空则表示第一列，如都为空则随机排序）
        /// </summary>
        public string PreEntityFieldName { get; }
        /// <summary>
        /// Excel列名
        /// </summary>
        public string ExcelColumnName { get; }
        /// <summary>
        /// 数据格式
        /// </summary>
        public string DataFormat { get; }
        /// <summary>
        /// 表头宽度
        /// </summary>
        public short Width { get; }
        /// <summary>
        /// 批注内容
        /// </summary>
        public string CommentContent { get; set; } = "";
        /// <summary>
        /// 批注作者
        /// </summary>
        public string CommentAuthor { get; set; } = "System";
    }
}
