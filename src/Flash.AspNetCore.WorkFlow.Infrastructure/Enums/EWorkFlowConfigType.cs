using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.Enums
{
    /// <summary>
    /// 流程配置类型
    /// </summary>
    public enum EWorkFlowConfigType
    {
        /// <summary>
        /// 合同
        /// </summary>
        [Description("合同")]
        Contract = 1,

        /// <summary>
        /// 审批中心
        /// </summary>
        [Description("审批中心")]
        ApprovalCenter = 2,
    }
}
