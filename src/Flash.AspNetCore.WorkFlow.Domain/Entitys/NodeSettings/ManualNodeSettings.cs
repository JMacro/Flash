using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeSettings
{
    /// <summary>
    /// 人工节点设置
    /// </summary>
    public class ManualNodeSettings : INodeSetting
    {
        /// <summary>
        /// 是否去重启用
        /// </summary>
        public bool DistinctEnable { get; set; }

        /// <summary>
        /// 去重模式
        /// </summary>
        public DistinctModeEnum DistinctMode { get; set; }

        /// <summary>
        /// 处理人为发起人自动通过
        /// </summary>
        public bool DistinctAutoSkipApplicant { get; set; }

        /// <summary>
        /// 启用超时提醒
        /// </summary>
        public bool ReminderEnable { get; set; }

        /// <summary>
        /// 保存为秒
        /// </summary>
        public long ReminderTimeSpan { get; set; }

        /// <summary>
        /// 显示单位
        /// </summary>
        public ReminderTimeSpanDisplayUnitEnum ReminderTimeSpanDisplayUnit { get; set; }

        /// <summary>
        /// 超市通知用户id
        /// </summary>
        public IEnumerable<string> ReminderNotifyUserIds { get; set; }

        /// <summary>
        /// 超时提醒发送目标数组(如,EMAIL，SMS)
        /// </summary>
        public IEnumerable<string> ReminderSendProviders { get; set; }

        /// <summary>
        /// 根据API判断是否发出提醒通知
        /// </summary>
        public bool EnableByAPI { get; set; }

        /// <summary>
        /// GET,POST,PUT,DELETE
        /// </summary>
        public string ReminderCanInformApiMethod { get; set; }

        /// <summary>
        /// 是否发出提醒通知的url
        /// </summary>
        public string ReminderCanInformApiUrl { get; set; }

        /// <summary>
        /// 是否发出提醒通知的header
        /// </summary>
        public IDictionary<string, string> ReminderCanInformApiHeader { get; set; }

        /// <summary>
        /// 是否发出提醒通知的body
        /// </summary>
        public string ReminderCanInformApiBody { get; set; }

        /// <summary>
        /// 媒体类型
        /// </summary>
        public string ReminderCanInformApiMediaType { get; set; }

        /// <summary>
        /// 请求返回结果索引器
        /// </summary>
        public string ReminderCanInformApiResponseSelector { get; set; }

        /// <summary>
        /// 超时通知的URL
        /// </summary>
        public string ReminderNotifyUrl { get; set; }

        /// <summary>
        /// 超时通知的方法
        /// </summary>
        public string ReminderNotifyMethod { get; set; }

        /// <summary>
        /// 超时通知的Header
        /// </summary>
        public IDictionary<string, string> ReminderNotifyHeader { get; set; }

        /// <summary>
        /// 超时通知的Body
        /// </summary>
        public string ReminderNotifyBody { get; set; }

        /// <summary>
        /// 媒体类型
        /// </summary>
        public string ReminderNotifyMediaType { get; set; }

        /// <summary>
        /// 是否开启结果提醒
        /// </summary>
        public bool HasResultRemind { get; set; }

        /// <summary>
        /// 审批结果提醒方式
        /// </summary>
        public ApprovalResultRemindTypeDTO ApprovalResultRemindType { get; set; }

        public enum ReminderTimeSpanDisplayUnitEnum
        {
            Hour = 0,
            Day = 1
        }

        public enum DistinctModeEnum
        {
            /// <summary>
            /// 只保留第一个
            /// </summary>
            OnlyKeepFirst = 0,

            /// <summary>
            /// 仅在连续时候去重
            /// </summary>
            OnlyDistincSubsequent = 1
        }

        public enum ApprovalResultRemindTypeDTO
        {
            /// <summary>
            /// 站内信
            /// </summary>
            InStation = 1
        }
    }
}
