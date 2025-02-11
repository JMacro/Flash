using System;
namespace Flash.Widgets.Models.TaobaoUtils
{
    public class CalculateByDFRequestData : ShopInfo
    {
        /// <summary>
        /// 销售订单文件Id
        /// </summary>
        public long FileIdByExportOrderInfo { get; set; }
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime? StatDate { get; set; }
    }
}

