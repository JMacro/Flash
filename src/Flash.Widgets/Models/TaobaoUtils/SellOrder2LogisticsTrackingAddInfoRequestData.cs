using System;
namespace Flash.Widgets.Models.TaobaoUtils
{
	public class SellOrder2LogisticsTrackingAddInfoRequestData : ShopInfo
    {
        /// <summary>
        /// 记录Id
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// 主订单编号
        /// </summary>
        public string? MainOrderNumber { get; set; }
        /// <summary>
        /// 商家备注
        /// </summary>
        public string? MerchantRemarks { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public string? ExpressNumber { get; set; }
        /// <summary>
        /// 物流公司
        /// </summary>
        public string? ExpressName { get; set; }
        /// <summary>
        /// 收件手机号
        /// </summary>
        public string? Phone { get; set; }
    }
}

