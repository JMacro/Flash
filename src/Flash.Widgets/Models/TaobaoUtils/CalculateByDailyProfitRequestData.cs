using System;
namespace Flash.Widgets.Models.TaobaoUtils
{
    public class CalculateByDailyProfitRequestData : ShopInfo
    {
        /// <summary>
        /// 销售订单文件Id
        /// </summary>
        public long FileIdByExportOrderInfo { get; set; }
        /// <summary>
        /// 订单打印日期
        /// </summary>
        public DateTime? StatDate { get; set; }
        /// <summary>
        /// 推广费
        /// </summary>
        public decimal? PromotionFee { get; set; } = 0;
        /// <summary>
        /// 代发费
        /// </summary>
        public decimal? DF_Fee { get; set; } = 0;
    }
}

