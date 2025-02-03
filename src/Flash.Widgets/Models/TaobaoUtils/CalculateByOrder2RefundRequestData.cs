using System;
namespace Flash.Widgets.Models.TaobaoUtils
{
	public class CalculateByOrder2RefundRequestData
	{
        /// <summary>
        /// 统计开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 统计结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 销售订单文件Id
        /// </summary>
        public long FileIdByExportOrderInfo { get; set; }
        /// <summary>
        /// 订单打印日志文件Id
        /// </summary>
        public long FileIdByOrderPrintLogger { get; set; }
    }
}

