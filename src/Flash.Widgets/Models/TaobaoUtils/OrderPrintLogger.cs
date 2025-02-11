using System;
using Flash.Extensions.Office;

namespace Flash.Widgets.Models.TaobaoUtils
{
    public class OrderPrintLogger
    {
        /// <summary>
        /// 打印Id
        /// </summary>
        public string PrintId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 快递公司
        /// </summary>
        public string ExpressName { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public string ExpressNumber { get; set; }
        /// <summary>
        /// 面单状态
        /// </summary>
        public string ExpressStatus { get; set; }
        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime PrintTime { get; set; }
        /// <summary>
        /// 打印日期
        /// </summary>
        public DateTime PrintDate => PrintTime.Date;
        /// <summary>
        /// 打印类型
        /// </summary>
        public string PrintType { get; set; }
        /// <summary>
        /// 商品信息
        /// </summary>
        public string ShopDetails { get; set; }

        public static List<ExcelHeaderColumn> GetHeaderColumns()
        {
            var printLoggerHeaderColumns = new List<ExcelHeaderColumn>();
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("打印批次号", nameof(OrderPrintLogger.PrintId)));
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("订单编号", nameof(OrderPrintLogger.OrderNumber), 25));
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("快递公司", nameof(OrderPrintLogger.ExpressName), 25));
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("运单号", nameof(OrderPrintLogger.ExpressNumber), 25));
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("面单状态", nameof(OrderPrintLogger.ExpressStatus), 25));
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("打印时间", nameof(OrderPrintLogger.PrintTime), "yyyy-MM-dd HH:mm:ss", 18));
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("打印类型", nameof(OrderPrintLogger.PrintType)));
            printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("商品信息", nameof(OrderPrintLogger.ShopDetails), 40));

            return printLoggerHeaderColumns;
        }
    }
}

