using Flash.Extensions.Office;

namespace Flash.Widgets.Configures
{
    public class SkuCodeConfigure
    {
		public List<SkuInfo> SkuCodes { get; set; }
	}

    public class SkuInfo
    {
        /// <summary>
        /// Sku编码
        /// </summary>
        public string SkuCode { get; set; }
        /// <summary>
        /// Sku编码别名
        /// </summary>
        public List<string> AliasCodes { get; set; } = new List<string>();
        /// <summary>
        /// Sku编码别名
        /// </summary>
        public string AliasCodesStr => string.Join(",", AliasCodes);
        /// <summary>
        /// Sku名称
        /// </summary>
        public string SkuName { get; set; }

        public static List<ExcelHeaderColumn> GetHeaderColumns()
        {
            var headerColumns = new List<ExcelHeaderColumn>();
            headerColumns.Add(ExcelHeaderColumn.Create("Sku名称", nameof(SkuInfo.SkuName), 25));
            headerColumns.Add(ExcelHeaderColumn.Create("Sku编码", nameof(SkuInfo.SkuCode), 25));
            headerColumns.Add(ExcelHeaderColumn.Create("Sku编码别名", nameof(SkuInfo.AliasCodesStr), 50));

            return headerColumns;
        }
    }
}

