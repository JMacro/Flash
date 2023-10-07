using System;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs;
using MediatR;
using System.Collections.Generic;

namespace Flash.AspNetCore.WorkFlow.Domain.Commands.FlowConfigs.UpdateFlowConfig
{
	internal class UpdateFlowConfigCommand : IRequest
    {
        public List<FlowConfigSaveData> FlowConfigs { get; set; }
    }
}

