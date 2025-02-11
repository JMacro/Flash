using System;
namespace Flash.Widgets.Models.TaobaoUtils
{
	public class CalculateByOrder2RefundResponseData
	{
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public string ExpressNumber { get; set; }
        /// <summary>
        /// 退款状态
        /// </summary>
        public string RefundStatus { get; set; }
        /// <summary>
        /// 买家实际支付金额
        /// </summary>
        public decimal BuyActualAmount { get; set; }
    }
}

