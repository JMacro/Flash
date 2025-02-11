using System;
using Flash.Extensions.Office;

namespace Flash.Widgets.Models.TaobaoUtils
{
    public class DaiFanYunDanInfo
    {
        /// <summary>
        /// 主订单编号
        /// </summary>
        public string MainOrderNumber { get; set; }
        /// <summary>
        /// 快递公司编码
        /// </summary>
        public string ExpressCode { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public string ExpressNumber { get; set; }

        public static List<ExcelHeaderColumn> GetHeaderColumns()
        {
            var headerColumns = new List<ExcelHeaderColumn>();
            headerColumns.Add(ExcelHeaderColumn.Create("主订单编号", nameof(DaiFanYunDanInfo.MainOrderNumber), 25));
            headerColumns.Add(ExcelHeaderColumn.Create("快递公司名称/编码", nameof(DaiFanYunDanInfo.ExpressCode), 25));
            headerColumns.Add(ExcelHeaderColumn.Create("运单号", nameof(DaiFanYunDanInfo.ExpressNumber), 25));

            return headerColumns;
        }
    }
}

