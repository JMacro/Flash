using System;
namespace Flash.Widgets.Models.TaobaoUtils
{
    public class ExportShopStaticRequestData : ShopInfo
    {
        /// <summary>
        /// 统计开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 统计结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 销售订单文件Id
        /// </summary>
        public long FileIdByExportOrderInfo { get; set; }
    }
}

