using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.DO
{
    public class SaveFlowTypeRequestDO
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 审批流名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 审批流描述
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 业务场景 Id
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 表单类型
        /// </summary>
        public short FormType { get; set; }

        /// <summary>
        /// 字段 Id
        /// </summary>
        public List<long> FieldIds { get; set; }

        /// <summary>
        /// 节点
        /// </summary>
        public ApprovalFlowNode RootNode { get; set; }

        /// <summary>
        /// 自动去重
        /// </summary>
        public EAutoRemoveRepeatType AutoRemoveRepeatType { get; set; }

        /// <summary>
        /// 审批人和发起人是同一个人，审批自动通过
        /// </summary>
        public bool AutoPass { get; set; }

        /// <summary>
        /// 超时提醒配置类
        /// </summary>
        public TimeoutRemindConfig TimeoutRemindConfig { get; set; }

        /// <summary>
        /// 拒绝审核的推荐回复
        /// </summary>
        public List<string> RefuseRecommendReplys { get; set; }

        /// <summary>
        /// 是否开启结果提醒
        /// </summary>
        public bool HasResultRemind { get; set; }

        /// <summary>
        /// 审批结果提醒方式
        /// </summary>
        public EApprovalResultRemindType ApprovalResultRemindType { get; set; }
    }

    /// <summary>
    /// 节点
    /// </summary>
    public class ApprovalFlowNode
    {
        /// <summary>
        /// 是否根节点
        /// </summary>
        public bool IsRoot { get; set; }

        /// <summary>
        /// 节点Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public ENodeType Type { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 审批类型
        /// </summary>
        public EApprovalTargetType TargetType { get; set; }

        /// <summary>
        /// 审批目标Id列表
        /// </summary>
        public List<string> TargetIds { get; set; } = new List<string>();

        /// <summary>
        /// 是否会签
        /// </summary>
        public bool JointlySign { get; set; }

        /// <summary>
        /// 是否允许转审
        /// </summary>
        public bool AllowTransferForApproval { get; set; }

        /// <summary>
        /// 是否需要人工检查
        /// </summary>
        public bool IsManualCheck { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<ApprovalFlowNode> ChildNodes { get; set; } = new List<ApprovalFlowNode>();

        /// <summary>
        /// 审批结果=>配置审批人(自动审批)专用,true为自动通过
        /// </summary>
        public bool IsAutoPass { get; set; }

        /// <summary>
        /// 条件组
        /// </summary>
        public List<ConditionGroup> ConditionGroups { get; set; } = new List<ConditionGroup>();

        /// <summary>
        /// 条件组字符串
        /// </summary>
        public string ConditionGroupString { get; set; }

        /// <summary>
        /// 条件节点的true或false
        /// </summary>
        public bool Falg { get; set; }

        /// <summary>
        /// 节点显示内容
        /// 用户名/角色名/上级主管
        /// </summary>
        public List<string> Contents { get; set; } = new List<string>();
    }

    /// <summary>
    /// 条件组
    /// </summary>
    public sealed class ConditionGroup
    {
        /// <summary>
        /// 对象类型
        /// </summary>
        public EConditionObjectType ObjectType { get; set; }

        /// <summary>
        /// 条件项
        /// </summary>
        public List<ConditionItem> Items { get; set; } = new List<ConditionItem>();
    }

    /// <summary>
    /// 条件项
    /// </summary>
    public sealed class ConditionItem
    {
        /// <summary>
        /// 对象类型
        /// </summary>
        public EConditionObjectType ObjectType { get; set; }

        /// <summary>
        /// 值，多个按逗号分割
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 字段数据类型
        /// </summary>
        public EWorkFlowFieldDataType FieldDataType { get; set; }
    }

    /// <summary>
    /// 超时提醒配置类
    /// </summary>
    public class TimeoutRemindConfig
    {
        /// <summary>
        /// 数值
        /// </summary>
        public decimal Number { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public EDateType DateType { get; set; }

        /// <summary>
        /// 法定节假日跳过
        /// </summary>
        public bool LegalHolidaysSkip { get; set; }

        /// <summary>
        /// 额外提醒人员
        /// </summary>
        public List<Guid> RemindUserIds { get; set; } = new List<Guid>();
    }

    /// <summary>
    /// 时间类型
    /// </summary>
    public enum EDateType
    {
        /// <summary>
        /// 天
        /// </summary>
        [Description("天")] Day = 1,

        /// <summary>
        /// 小时
        /// </summary>
        [Description("小时")] Hour = 2
    }
}
