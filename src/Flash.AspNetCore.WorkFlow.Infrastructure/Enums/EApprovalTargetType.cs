using System.ComponentModel;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.Enums
{
    /// <summary>
    /// 审批目标类型
    /// </summary>
    public enum EApprovalTargetType
    {
        /// <summary>
        /// 指定审批人
        /// </summary>
        [Description("指定审批人")]
        User = 1,

        /// <summary>
        /// 指定角色
        /// </summary>
        [Description("指定角色")]
        Role = 2,

        /// <summary>
        /// 指定主管/领导
        /// </summary>
        [Description("指定主管/领导")]
        Leader = 3
    }
}
