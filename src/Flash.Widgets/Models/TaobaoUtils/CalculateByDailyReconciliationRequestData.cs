namespace Flash.Widgets.Models.TaobaoUtils
{
    public class CalculateByDailyReconciliationRequestData : ShopInfo
    {
        /// <summary>
        /// 订单打印日志文件Id
        /// </summary>
        public long FileIdByOrderPrintLogger { get; set; }
        /// <summary>
        /// 销售订单文件Id
        /// </summary>
        public long FileIdByExportOrderInfo { get; set; }
        /// <summary>
        /// 订单打印日期
        /// </summary>
        public DateTime? OrderPrintDate { get; set; }
    }
}

