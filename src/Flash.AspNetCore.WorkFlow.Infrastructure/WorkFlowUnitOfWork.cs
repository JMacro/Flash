using System.Threading.Tasks;
using Flash.AspNetCore.WorkFlow.Infrastructure.DomainEventsDispatching;
using Flash.Extensions.ORM.EntityFrameworkCore;

namespace Flash.AspNetCore.WorkFlow.Infrastructure
{
    public class WorkFlowUnitOfWork : UnitOfWork<WorkFlowDbContext>, IWorkFlowUnitOfWork
    {
        private readonly IDomainEventsDispatcher _domainEventsDispatcher;

        public WorkFlowUnitOfWork(WorkFlowDbContext context, IDomainEventsDispatcher domainEventsDispatcher) :base(context)
		{
            this._domainEventsDispatcher = domainEventsDispatcher;
        }

        public override bool SaveChanges()
        {
            this._domainEventsDispatcher.DispatchEventsAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return base.SaveChanges();
        }

        public override async Task<bool> SaveChangesAsync()
        {
            await this._domainEventsDispatcher.DispatchEventsAsync();
            return await base.SaveChangesAsync();
        }
    }
}

