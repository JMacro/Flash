using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Flash.Extensions.ORM;

namespace Flash.Widgets.Models.TaobaoUtils
{
	public class SellOrder2LogisticsTrackingImportRequestData : ShopInfo
    {
        /// <summary>
        /// 销售订单文件Id
        /// </summary>
        public long FileIdByExportOrderInfo { get; set; }
    }

    public class SellOrder2LogisticsTrackingResponseData : ShopInfo
    {
        /// <summary>
        /// 主订单编号
        /// </summary>
        public string MainOrderNumber { get; set; }
        /// <summary>
        /// 商家备注
        /// </summary>
        public string MerchantRemarks { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public string ExpressNumber { get; set; }
        /// <summary>
        /// 物流公司
        /// </summary>
        public string ExpressName { get; set; }
    }

    /// <summary>
    /// 淘宝物流轨迹跟踪
    /// </summary>
    [Table("taobao_logistics_tracking")]
    public class LogisticsTrackingEntity : IEntity<long>
    {
        /// <summary>
        /// 店铺Id
        /// </summary>
        public string ShopId { get; set; }
        /// <summary>
        /// 主订单编号
        /// </summary>
        public string MainOrderNumber { get; set; }
        /// <summary>
        /// 商家备注
        /// </summary>
        public string MerchantRemarks { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public string ExpressNumber { get; set; }
        /// <summary>
        /// 物流公司
        /// </summary>
        public string ExpressName { get; set; }
        /// <summary>
        /// 收件手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public LogisticsTrackingState State { get; set; }
        /// <summary>
        /// 物流信息
        /// </summary>
        public string LogisticsTracking { get; set; } = "";
        /// <summary>
        /// 业务发生时间
        /// </summary>
        public DateTime? BusinessTime { get; set; }
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; }
        /// <summary>
        /// 系统备注
        /// </summary>
        public string? SystemRemarks { get; set; } = "";

        public long Id { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateTime { get; set; }
        public long CreateUserId { get; set; }
        public DateTime LastModifyTime { get; set; }
        public long LastModifyUserId { get; set; }
    }

    public enum LogisticsTrackingState
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Description("待处理")]
        WaitHandle = 0,
        /// <summary>
        /// 系统跟进中
        /// </summary>
        [Description("系统跟进中")]
        SystemTracking = 1,
        /// <summary>
        /// 完成签收
        /// </summary>
        [Description("完成签收")]
        CompletedSign = 2,
        /// <summary>
        /// 停滞
        /// </summary>
        [Description("停滞")]
        Stagnation = 3,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        CompletedTrack = 4,
        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        Error = 5
    }

}

