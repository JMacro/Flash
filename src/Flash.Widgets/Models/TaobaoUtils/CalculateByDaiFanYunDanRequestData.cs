using System;
namespace Flash.Widgets.Models.TaobaoUtils
{
    public class CalculateByDaiFanYunDanRequestData
    {
        /// <summary>
        /// 代发文件Id
        /// </summary>
        public long FileIdByDaiFa { get; set; }
        /// <summary>
        /// 打单日志文件Id
        /// </summary>
        public long FileIdByOrderPrint { get; set; }
        /// <summary>
        /// 快递公司编码
        /// </summary>
        public string ExpressCode { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? Date { get; set; }
    }
}

