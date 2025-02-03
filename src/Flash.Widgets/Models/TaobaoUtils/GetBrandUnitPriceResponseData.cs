using System;
namespace Flash.Widgets.Models.TaobaoUtils
{
    public class GetBrandUnitPriceResponseData
    {
        public string CacheName { get; set; }
        /// <summary>
        /// 快递费
        /// </summary>
        public decimal ExpressDelivery { get; set; }
        public List<BrandUnitPriceItem> BrandUnitPriceItems { get; set; }
    }

    public class BrandUnitPriceItem
    {
        public string BrandCode { get; set; }
        public decimal Price { get; set; }
    }
}

