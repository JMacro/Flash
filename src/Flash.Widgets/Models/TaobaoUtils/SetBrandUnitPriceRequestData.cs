using System;
using Flash.Widgets.Controllers;

namespace Flash.Widgets.Models.TaobaoUtils
{
    public class SetBrandUnitPriceRequestData : ShopInfo
    {
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime? Date { get; set; }
        public string CacheName { get; set; }
        /// <summary>
        /// 快递费
        /// </summary>
        public decimal ExpressDelivery { get; set; }
        public List<BrandUnitPriceItem> BrandUnitPriceItems { get; set; }
    }

    public class GetBrandUnitPriceRequestData : ShopInfo
    {
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime? Date { get; set; }
        public string CacheName { get; set; }
    }
}

