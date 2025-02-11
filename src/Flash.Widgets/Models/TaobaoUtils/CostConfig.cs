namespace Flash.Widgets.Models.TaobaoUtils
{
    public class CostConfig
    {
        /// <summary>
        /// 快递费
        /// </summary>
        public decimal ExpressDelivery { get; set; }
        /// <summary>
        /// 品牌单价
        /// </summary>
        public Dictionary<string, decimal> BrandUnitPrices { get; set; }

        public static CostConfig Convert(GetBrandUnitPriceResponseData data)
        {
            var result = new CostConfig
            {
                ExpressDelivery = data.ExpressDelivery,
                BrandUnitPrices = new Dictionary<string, decimal>()
            };

            foreach (var item in data.BrandUnitPriceItems)
            {
                result.BrandUnitPrices.TryAdd(item.BrandCode, item.Price);
            }

            return result;
        }
    }
}

