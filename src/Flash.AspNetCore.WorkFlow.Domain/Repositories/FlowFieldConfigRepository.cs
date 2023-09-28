﻿using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Repositories;
using Flash.AspNetCore.WorkFlow.Infrastructure;
using Flash.AspNetCore.WorkFlow.Infrastructure.PO;
using Flash.Extensions.ORM.EntityFrameworkCore;

namespace Flash.AspNetCore.WorkFlow.Domain.Repositories
{
    internal sealed class FlowFieldConfigRepository : Repository<FlowFieldConfigPO>, IFlowFieldConfigRepository
    {
        public FlowFieldConfigRepository(WorkFlowDbContext context) : base(context)
        {
        }
    }
}