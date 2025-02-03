using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.Office
{
    /// <summary>
    /// Excel列映射
    /// </summary>
    public class ExcelColumnMap
    {
        /// <summary>
        /// Excel列名
        /// </summary>
        public string ExcelColumnName { get; set; }
        /// <summary>
        /// 实体字段名称
        /// </summary>
        public string EntityFieldName { get; set; }

        /// <summary>
        /// 创建映射关系
        /// </summary>
        /// <param name="excelColumnName">Excel列名</param>
        /// <param name="entityFieldName">实体字段名称</param>
        /// <returns></returns>
        public static ExcelColumnMap Create(string excelColumnName, string entityFieldName)
        {
            return new ExcelColumnMap { ExcelColumnName = excelColumnName, EntityFieldName = entityFieldName };
        }
    }
}
