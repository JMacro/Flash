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
        /// 
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
    }
}
