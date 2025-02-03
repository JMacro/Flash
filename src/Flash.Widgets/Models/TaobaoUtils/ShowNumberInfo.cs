using System;
using Flash.Extensions.Office;

namespace Flash.Widgets.Models.TaobaoUtils
{
    public class ShowNumberInfo
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ShopName { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        public string ShopCode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 成本额
        /// </summary>
        public decimal CostAmount { get; set; } = 0;
        /// <summary>
        /// 销售额
        /// </summary>
        public decimal SellAmount { get; set; }
        /// <summary>
        /// 利润额
        /// </summary>
        public decimal ProfitAmount { get; set; }
        /// <summary>
        /// Sku编码别名
        /// </summary>
        public List<string> AliasCodes { get; set; } = new List<string>();

        public static List<ExcelHeaderColumn> GetHeaderColumns()
        {
            var headerColumns = new List<ExcelHeaderColumn>();
            headerColumns.Add(ExcelHeaderColumn.Create("SKU名称", nameof(ShowNumberInfo.ShopName), 20));
            headerColumns.Add(ExcelHeaderColumn.Create("SKU编码", nameof(ShowNumberInfo.ShopCode), 20));
            headerColumns.Add(ExcelHeaderColumn.Create("数量", nameof(ShowNumberInfo.Number), 10));
            headerColumns.Add(ExcelHeaderColumn.Create("单价", nameof(ShowNumberInfo.UnitPrice), 10));
            headerColumns.Add(ExcelHeaderColumn.Create("成本额", nameof(ShowNumberInfo.CostAmount), 15, ExcelComment.Create("成本额=数量✖单价", "JMacro")));
            headerColumns.Add(ExcelHeaderColumn.Create("销售额", nameof(ShowNumberInfo.SellAmount), 15, ExcelComment.Create("客户成交金额", "JMacro")));
            headerColumns.Add(ExcelHeaderColumn.Create("利润额", nameof(ShowNumberInfo.ProfitAmount), 15, ExcelComment.Create("利润额=销售额-成本额", "JMacro")));

            return headerColumns;
        }

        /// <summary>
        /// 获得SKU统计表头
        /// </summary>
        /// <returns></returns>
        public static List<ExcelHeaderColumn> GetHeaderColumnsByStatic()
        {
            var headerColumns = new List<ExcelHeaderColumn>();
            headerColumns.Add(ExcelHeaderColumn.Create("Sku名称", nameof(ShowNumberInfo.ShopName), 25));
            headerColumns.Add(ExcelHeaderColumn.Create("数量", nameof(ShowNumberInfo.Number), 10));
            return headerColumns;
        }
    }
}

