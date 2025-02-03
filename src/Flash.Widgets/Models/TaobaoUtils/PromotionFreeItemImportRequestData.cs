using System;
using System.ComponentModel.DataAnnotations.Schema;
using Flash.Extensions.Office;

namespace Flash.Widgets.Models.TaobaoUtils
{
	public class PromotionFreeItemImportRequestData : ShopInfo
    {
        /// <summary>
        /// 文件Id
        /// </summary>
        public long FileId { get; set; }
    }

    /// <summary>
    /// 推广费明细
    /// </summary>
    [Table("taobao_promotion_free_item")]
    public class PromotionFreeItemEntity
    {
        /// <summary>
        /// 记账时间
        /// </summary>
        public DateTime AccountTime { get; set; }

        /// <summary>
        /// 交易日期
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// 收支类型
        /// </summary>
        public string IncomeExpenseType { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// 操作金额(元)
        /// </summary>
        public decimal OperationAmount { get; set; }

        /// <summary>
        /// 操作后余额(元)
        /// </summary>
        public decimal BalanceAfterOperation { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }


        public static List<ExcelHeaderColumn> GetHeaderColumns()
        {
            var printLoggerHeaderColumns = new List<ExcelHeaderColumn>();
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("记账时间", nameof(PromotionFreeItemEntity.AccountTime)));
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("交易日期", nameof(PromotionFreeItemEntity.TransactionDate)));
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("收支类型", nameof(PromotionFreeItemEntity.IncomeExpenseType)));
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("交易类型", nameof(PromotionFreeItemEntity.TransactionType)));
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("操作金额(元)", nameof(PromotionFreeItemEntity.OperationAmount)));
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("操作后余额(元)", nameof(PromotionFreeItemEntity.BalanceAfterOperation)));
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("备注", nameof(PromotionFreeItemEntity.Remarks)));

            return printLoggerHeaderColumns;
        }
    }
}

