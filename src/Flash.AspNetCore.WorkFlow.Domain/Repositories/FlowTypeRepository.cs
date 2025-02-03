using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Repositories;
using Flash.AspNetCore.WorkFlow.Infrastructure;
using Flash.AspNetCore.WorkFlow.Infrastructure.PO;
using Flash.Extensions.ORM.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Repositories
{
    public class FlowTypeRepository : Repository<FlowTypePO>, IFlowTypeRepository
    {
        public FlowTypeRepository(WorkFlowDbContext context) : base(context)
        {
        }
    }
}
