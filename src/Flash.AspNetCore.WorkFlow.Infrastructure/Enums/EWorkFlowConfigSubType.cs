using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.Enums
{
    public enum EWorkFlowConfigSubType
    {
        /// <summary>
        /// 业务模块
        /// </summary>
        [Description("业务模块")]
        BusinessModule = 1,

        /// <summary>
        /// 业务场景
        /// </summary>
        [Description("业务场景")]
        BusinessScenario = 2,

        /// <summary>
        /// 流程类型
        /// </summary>
        [Description("流程类型")]
        FlowType = 3,
    }
}
