using System.ComponentModel;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.Enums
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public enum ENodeType
    {
        /// <summary>
        /// 人工审批节点
        /// </summary>
        [Description("人工审批节点")]
        ManualApprovalNode = 1,

        /// <summary>
        /// 系统审批节点
        /// </summary>
        [Description("系统审批节点")]
        SystemApprovalNode = 2,

        /// <summary>
        /// 抄送节点
        /// </summary>
        [Description("抄送节点")]
        CcNode = 3,

        /// <summary>
        /// 条件节点
        /// </summary>
        [Description("条件节点")]
        ConditionNode = 4,
    }
}
