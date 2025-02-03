using System;
using Flash.Extensions.Office;

namespace Flash.Widgets.Models.TaobaoUtils
{
    public class ExportOrderInfo
    {
        /// <summary>
        /// 主订单编号
        /// </summary>
        public string MainOrderNumber { get; set; }
        /// <summary>
        /// 购买数量
        /// </summary>
        public int BuyNumber { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderStatus { get; set; }
        /// <summary>
        /// 商家编码(商品编码)
        /// </summary>
        public string ShopCode { get; set; }
        /// <summary>
        /// 买家实际支付金额
        /// </summary>
        public decimal BuyActualAmount { get; set; }
        /// <summary>
        /// 退款状态
        /// </summary>
        public string RefundStatus { get; set; }
        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime OrderCreateTime { get; set; }
        /// <summary>
        /// 订单付款时间
        /// </summary>
        public DateTime OrderPaymentTime { get; set; }
        /// <summary>
        /// 订单付款日期
        /// </summary>
        public DateTime OrderPaymentDate => OrderPaymentTime.Date;
        /// <summary>
        /// 商家备注
        /// </summary>
        public string MerchantRemarks { get; set; }
        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime DeliveryTime { get; set; }
        /// <summary>
        /// 发货日期
        /// </summary>
        public DateTime DeliveryDate => DeliveryTime.Date;
        /// <summary>
        /// 运单号
        /// </summary>
        public string ExpressNumberStr { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public string ExpressNumber => (!string.IsNullOrWhiteSpace(this.ExpressNumberStr) && this.ExpressNumberStr != "null") ? (this.ExpressNumberStr.Replace("No:", "")) : "";
        /// <summary>
        /// 物流公司
        /// </summary>
        public string ExpressName { get; set; }

        public static List<ExcelHeaderColumn> GetHeaderColumns()
        {
            var exportOrderHeaderColumns = new List<ExcelHeaderColumn>();
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("主订单编号", nameof(ExportOrderInfo.MainOrderNumber), 20));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("购买数量", nameof(ExportOrderInfo.BuyNumber)));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("订单状态", nameof(ExportOrderInfo.OrderStatus)));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("商家编码", nameof(ExportOrderInfo.ShopCode)));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("买家实际支付金额", nameof(ExportOrderInfo.BuyActualAmount)));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("退款状态", nameof(ExportOrderInfo.RefundStatus)));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("订单创建时间", nameof(ExportOrderInfo.OrderCreateTime), "yyyy-MM-dd HH:mm:ss", 18));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("订单付款时间", nameof(ExportOrderInfo.OrderPaymentTime), "yyyy-MM-dd HH:mm:ss", 18));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("商家备注", nameof(ExportOrderInfo.MerchantRemarks)));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("发货时间", nameof(ExportOrderInfo.DeliveryTime), "yyyy-MM-dd HH:mm:ss", 18));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("物流单号", nameof(ExportOrderInfo.ExpressNumberStr)));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("物流公司", nameof(ExportOrderInfo.ExpressName)));

            return exportOrderHeaderColumns;
        }
    }
}

