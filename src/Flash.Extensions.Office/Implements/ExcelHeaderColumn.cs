using System;
using System.Collections.Generic;

namespace Flash.Extensions.Office
{
    /// <summary>
    /// Excel表头列
    /// </summary>
    public class ExcelHeaderColumn
    {
        /// <summary>
        /// 列映射关系
        /// </summary>
        public ExcelColumnMap ColumnMap { get; set; }
        /// <summary>
        /// 数据格式
        /// </summary>
        public string DataFormat { get; set; }

        /// <summary>
        /// 创建Excel表头列
        /// </summary>
        /// <param name="excelColumnName">Excel列名</param>
        /// <param name="entityFieldName">实体字段名称</param>
        /// <param name="dataFormat">数据格式</param>
        /// <returns></returns>
        public static ExcelHeaderColumn Create(string excelColumnName, string entityFieldName, string dataFormat = "")
        {
            return new ExcelHeaderColumn
            {
                ColumnMap = ExcelColumnMap.Create(excelColumnName, entityFieldName),
                DataFormat = dataFormat
            };
        }

        /// <summary>
        /// 创建Excel表头列，需为属性打上<see cref="ExcelHeaderAttribute"/>标签
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<ExcelHeaderColumn> Create<T>() where T : class, new()
        {
            var objType = typeof(T);
            var properties = objType.GetProperties();
            var sortField = new List<string>();

            Func<Attribute[], Tuple<bool, ExcelHeaderAttribute>> IsMyAttribute = o =>
            {
                foreach (Attribute a in o)
                {
                    if (a is ExcelHeaderAttribute)
                        return Tuple.Create(true, a as ExcelHeaderAttribute);
                }
                return Tuple.Create(false, default(ExcelHeaderAttribute));
            };

            var dictExcelHeaderColumn = new Dictionary<string, ExcelHeaderColumn>();
            foreach (var item in properties)
            {
                var attributeResult = IsMyAttribute(System.Attribute.GetCustomAttributes(item, true));
                if (!attributeResult.Item1) continue;

                var index = sortField.IndexOf(attributeResult.Item2.PreEntityFieldName);
                if (index < 0)
                {
                    sortField.Insert(0, item.Name);
                }
                else
                {
                    sortField.Insert(index + 1, item.Name);
                }
                dictExcelHeaderColumn.Add(item.Name, Create(attributeResult.Item2.ExcelColumnName, item.Name, attributeResult.Item2.DataFormat));
            }

            var result = new List<ExcelHeaderColumn>();
            foreach (var fieldName in sortField)
            {
                if (dictExcelHeaderColumn.TryGetValue(fieldName, out ExcelHeaderColumn value))
                {
                    result.Add(value);
                }
            }
            return result;
        }
    }
}
