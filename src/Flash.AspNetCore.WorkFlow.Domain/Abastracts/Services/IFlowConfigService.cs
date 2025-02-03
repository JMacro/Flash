using Flash.AspNetCore.WorkFlow.Domain.DO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flash.AspNetCore.WorkFlow.Domain.Abastracts.Services
{
    public interface IFlowConfigService
    {
        Task Init(List<InitFlowConfigRequestDO> initFlowConfigs);
    }
}
