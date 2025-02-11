using Flash.AspNetCore.WorkFlow.Application.Dtos.FlowConfigs;
using System;
using System.Collections.Generic;

namespace Flash.AspNetCore.WorkFlow.Application.Abastracts.Services
{
    /// <summary>
    /// 工作流配置(模块与场景)
    /// </summary>
    public interface IFlowConfigService
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="func"></param>
        void Init(Func<IEnumerable<FlowConfigRequestDto>> func);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <returns></returns>
        List<FlowConfigTreeResponseDto> GetFlowConfigs();
    }
}
