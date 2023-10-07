using System.Collections.Generic;
using System.Linq;
using Flash.AspNetCore.WorkFlow.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.DomainEventsDispatching
{
    public class DomainEventsAccessor : IDomainEventsAccessor
    {
        private readonly WorkFlowDbContext _dbContext;

        public DomainEventsAccessor(WorkFlowDbContext dbContext)
        {
            _dbContext = dbContext;
#if DEBUG
            var logger = MicrosoftContainer.Instance.GetService<ILogger<DomainEventsAccessor>>();
            logger.LogInformation($"DbContextId:{_dbContext.ContextId}");
#endif
        }

        public IReadOnlyCollection<IDomainEvent> GetAllDomainEvents()
        {
            var domainEntities = this._dbContext.ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any()).ToList();

            return domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();
        }

        public void ClearAllDomainEvents()
        {
            var domainEntities = this._dbContext.ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any()).ToList();

            domainEntities
                .ForEach(entity => entity.Entity.ClearDomainEvents());
        }
    }
}
