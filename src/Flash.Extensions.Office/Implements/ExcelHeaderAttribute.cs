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
        /// 
        /// </summary>
        /// <param name="preEntityFieldName">上一个实体字段名称，用于表头排序（为空则表示第一列，如都为空则随机排序）</param>
        /// <param name="excelColumnName">Excel列名</param>
        /// <param name="dataFormat">数据格式</param>
        public ExcelHeaderAttribute(string preEntityFieldName, string excelColumnName, string dataFormat = "")
        {
            PreEntityFieldName = preEntityFieldName;
            ExcelColumnName = excelColumnName;
            DataFormat = dataFormat;
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
    }
}
