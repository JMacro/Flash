using System;
using Flash.Extensions;

namespace Flash.Widgets.Models.TaobaoUtils
{
	public class SellOrder2LogisticsTrackingGetListRequestData : ShopInfo, IPageQuery
    {
		public SellOrder2LogisticsTrackingGetListRequestData()
		{
		}

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public PageOrderBy OrderBy { get; set; }

        /// <summary>
        /// 主订单编号
        /// </summary>
        public string MainOrderNumber { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public string ExpressNumber { get; set; }
        /// <summary>
        /// 物流公司
        /// </summary>
        public string ExpressName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public LogisticsTrackingState? State { get; set; }
    }

	public class SellOrder2LogisticsTrackingGetListResponseData: LogisticsTrackingEntity
    {
        /// <summary>
        /// 状态
        /// </summary>
        public string StateStr => this.State.GetEnumDescript();
    }

}

