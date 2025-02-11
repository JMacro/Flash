using System;
using Flash.Extensions.Office;

namespace Flash.Widgets.Models.TaobaoUtils
{
	public class ShunfenReconciliationRequestData : ShopInfo
    {
        /// <summary>
        /// 订单打印日志文件Id
        /// </summary>
        public long FileIdByOrderPrintLogger { get; set; }
        /// <summary>
        /// 顺丰快递对账单文件Id
        /// </summary>
        public long FileIdByShunfen { get; set; }
    }

    public class ShunfenReconciliationInfo
    {
        /// <summary>
        /// 运单号
        /// </summary>
        public string ExpressNumber { get; set; }
        /// <summary>
        /// 寄件地区
        /// </summary>
        public string AddressFrom { get; set; }
        /// <summary>
        /// 到件地区
        /// </summary>
        public string AddressTo { get; set; }
        /// <summary>
        /// 计费重量
        /// </summary>
        public decimal Weight { get; set; }
        /// <summary>
        /// 应付金额
        /// </summary>
        public decimal PayableAmount { get; set; }
        /// <summary>
        /// 服务类型
        /// </summary>
        public string ServerType { get; set; }

        public static List<ExcelHeaderColumn> GetHeaderColumns()
        {
            var exportOrderHeaderColumns = new List<ExcelHeaderColumn>();
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("运单号码", nameof(ShunfenReconciliationInfo.ExpressNumber), 20));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("寄件地区", nameof(ShunfenReconciliationInfo.AddressFrom), 20));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("到件地区", nameof(ShunfenReconciliationInfo.AddressTo), 20));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("计费重量", nameof(ShunfenReconciliationInfo.Weight), 20));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("应付金额", nameof(ShunfenReconciliationInfo.PayableAmount), 20));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("服务", nameof(ShunfenReconciliationInfo.ServerType), 20));

            return exportOrderHeaderColumns;
        }
    }

    public class ShunfenReconciliationState
    {
        /// <summary>
        /// 总行数
        /// </summary>
        public int TotalRowNumber { get; set; }
        /// <summary>
        /// 总面单数(去重)
        /// </summary>
        public int TotalExpressNumber { get; set; }
        /// <summary>
        /// 总应付金额
        /// </summary>
        public decimal TotalPayableAmount{ get; set; }

        public static List<ExcelHeaderColumn> GetHeaderColumns()
        {
            var exportOrderHeaderColumns = new List<ExcelHeaderColumn>();
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("总行数", nameof(ShunfenReconciliationState.TotalRowNumber), 20));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("总面单数(去重)", nameof(ShunfenReconciliationState.TotalExpressNumber), 20));
            exportOrderHeaderColumns.Add(ExcelHeaderColumn.Create("总应付金额", nameof(ShunfenReconciliationState.TotalPayableAmount)));

            return exportOrderHeaderColumns;
        }
    }
}

