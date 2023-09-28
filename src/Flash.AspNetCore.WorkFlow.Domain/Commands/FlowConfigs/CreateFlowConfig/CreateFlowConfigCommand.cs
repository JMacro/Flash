using Flash.AspNetCore.WorkFlow.Domain.Entitys.FieldConfigs;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs;
using MediatR;
using System.Collections.Generic;

namespace Flash.AspNetCore.WorkFlow.Domain.Commands.FlowConfigs.CreateFlowConfig
{
    internal class CreateFlowConfigCommand : IRequest
    {
        public List<FlowConfigCreateData> FlowConfigs { get; set; }
        public List<FieldConfigSaveData> FieldConfigs { get; set; }
    }
}
