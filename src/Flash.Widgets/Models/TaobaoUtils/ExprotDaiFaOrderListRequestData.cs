using Flash.Extensions.Office;

namespace Flash.Widgets.Models.TaobaoUtils
{
    public class ExprotDaiFaOrderListRequestData
    {
        public List<ExprotDaiFaOrderItem> Items { get; set; }
    }

    public class ExprotDaiFaOrderItem
    {
        /// <summary>
        /// 收件人(必填)
        /// </summary>
        public string Recipient { get; set; } = "";
        /// <summary>
        /// 手机号(必填)
        /// </summary>
        public string PhoneNumber { get; set; } = "";
        /// <summary>
        /// 收货地址(必填)
        /// </summary>
        public string Address { get; set; } = "";
        /// <summary>
        /// 平台订单号（非必填）
        /// </summary>
        public string PlatformOrderNumber { get; set; } = "";
        /// <summary>
        /// 商品信息(非必填)
        /// </summary>
        public string Commodity { get; set; } = "";
        /// <summary>
        /// 规格信息(非必填)
        /// </summary>
        public string Specification { get; set; } = "";
        /// <summary>
        /// 商品数量(非必填)
        /// </summary>
        public string CommodityNumber { get; set; } = "";
        /// <summary>
        /// 重量kg(非必填)
        /// </summary>
        public string CommodityWeight { get; set; } = "";
        /// <summary>
        /// 备注(非必填)
        /// </summary>
        public string Remarks { get; set; }

        public static List<ExcelHeaderColumn> GetHeaderColumns()
        {
            var headerColumns = new List<ExcelHeaderColumn>();
            headerColumns.Add(ExcelHeaderColumn.Create("收件人(必填)", nameof(ExprotDaiFaOrderItem.Recipient)));
            headerColumns.Add(ExcelHeaderColumn.Create("手机号(必填)", nameof(ExprotDaiFaOrderItem.PhoneNumber), 25));
            headerColumns.Add(ExcelHeaderColumn.Create("收货地址(必填)", nameof(ExprotDaiFaOrderItem.Address), 18));
            headerColumns.Add(ExcelHeaderColumn.Create("平台订单号（非必填）", nameof(ExprotDaiFaOrderItem.PlatformOrderNumber)));
            headerColumns.Add(ExcelHeaderColumn.Create("商品信息(非必填)", nameof(ExprotDaiFaOrderItem.Commodity), 40));
            headerColumns.Add(ExcelHeaderColumn.Create("规格信息(非必填)", nameof(ExprotDaiFaOrderItem.Specification), 40));
            headerColumns.Add(ExcelHeaderColumn.Create("商品数量(非必填)", nameof(ExprotDaiFaOrderItem.CommodityNumber), 40));
            headerColumns.Add(ExcelHeaderColumn.Create("重量kg(非必填)", nameof(ExprotDaiFaOrderItem.CommodityWeight), 40));
            headerColumns.Add(ExcelHeaderColumn.Create("备注(非必填)", nameof(ExprotDaiFaOrderItem.Remarks), 40));

            return headerColumns;
        }
    }
}

