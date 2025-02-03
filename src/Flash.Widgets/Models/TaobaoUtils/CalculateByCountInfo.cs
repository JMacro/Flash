using System;
using Flash.Extensions.Office;

namespace Flash.Widgets.Models.TaobaoUtils
{
    public class CalculateByCountInfo
    {
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
            statHeaderColumns.Add(ExcelHeaderColumn.Create("SKU总数量", nameof(CalculateByCountInfo.SkuNumber), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("快递单数", nameof(CalculateByCountInfo.FaceSheetNumber), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("快递费", nameof(CalculateByCountInfo.FaceSheetAmount), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("推广费用", nameof(CalculateByCountInfo.PromotionFee), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("代发费用", nameof(CalculateByCountInfo.DF_Fee), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("总成本额", nameof(CalculateByCountInfo.CostAmount), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("总销售额", nameof(CalculateByCountInfo.SellAmount), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("总利润额", nameof(CalculateByCountInfo.ProfitAmount), 15, ExcelComment.Create("总利润额=总销售额-总成本额-快递费-推广费用-代发费用", "JMacro")));

            return statHeaderColumns;
        }

    }
}

