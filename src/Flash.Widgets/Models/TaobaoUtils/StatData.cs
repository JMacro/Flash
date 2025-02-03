using System;
using Flash.Extensions.Office;

namespace Flash.Widgets.Models.TaobaoUtils
{
    /// <summary>
    /// 统计数据
    /// </summary>
    public class StatData
    {
        /// <summary>
        /// 统计日期
        /// </summary>
        public string StatDate { get; set; }
        /// <summary>
        /// SKU数量
        /// </summary>
        public int SkuNumber { get; set; }
        /// <summary>
        /// 面单数（快递单数）
        /// </summary>
        public int FaceSheetNumber { get; set; }
        /// <summary>
        /// 面单总额
        /// </summary>
        public decimal FaceSheetAmount { get; set; }
        /// <summary>
        /// 成本总额
        /// </summary>
        public decimal CostAmount { get; set; }
        /// <summary>
        /// 销售总额
        /// </summary>
        public decimal SellAmount { get; set; }
        /// <summary>
        /// 利润总额
        /// </summary>
        public decimal ProfitAmount { get; set; }
        /// <summary>
        /// 总支出额
        /// </summary>
        public decimal TotalExpenditureAmount { get; set; }
        /// <summary>
        /// 快递费单价
        /// </summary>
        public decimal ExpressDeliveryUnitPrice { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 推广费
        /// </summary>
        public decimal PromotionFee { get; set; }
        /// <summary>
        /// 代发费
        /// </summary>
        public decimal DF_Fee { get; set; }

        public static List<ExcelHeaderColumn> GetHeaderColumns()
        {
            var statHeaderColumns = new List<ExcelHeaderColumn>();
            statHeaderColumns.Add(ExcelHeaderColumn.Create("统计日期", nameof(StatData.StatDate), "yyyy-MM-dd", 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("SKU总数量", nameof(StatData.SkuNumber), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("快递单数", nameof(StatData.FaceSheetNumber), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("快递费", nameof(StatData.FaceSheetAmount), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("快递费单价", nameof(StatData.ExpressDeliveryUnitPrice), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("推广费用", nameof(StatData.PromotionFee), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("代发费用", nameof(StatData.DF_Fee), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("总成本额", nameof(StatData.CostAmount), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("总销售额", nameof(StatData.SellAmount), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("总利润额", nameof(StatData.ProfitAmount), 15, ExcelComment.Create("总利润额=总销售额-总成本额-快递费-推广费用-代发费用", "JMacro")));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("总支出额", nameof(StatData.TotalExpenditureAmount), 15, ExcelComment.Create("总支出额=快递费+总成本额", "JMacro")));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("备注", nameof(StatData.Remarks), 15));

            return statHeaderColumns;
        }
    }
}

