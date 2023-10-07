using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Flash.AspNetCore.WorkFlow.Infrastructure.Exceptions;
using Flash.Extensions.ORM;
using Flash.Extensions.UidGenerator;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.Core
{
    /// <summary>
    /// 聚合根
    /// </summary>
    public abstract class AggregateRoot : BaseEntity<long>
    {
        private readonly List<IDomainEvent> _domainEvents;

        [NotMapped]
        public bool Removed { get; protected set; }
        [NotMapped]
        public int Version { get; protected set; }
        [NotMapped]
        public long AggregateId { get; protected set; }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        protected readonly IUniqueIdGenerator _uniqueIdGenerator;

        protected AggregateRoot()
        {
            _domainEvents = new List<IDomainEvent>();
            _uniqueIdGenerator = MicrosoftContainer.Instance.GetService<IUniqueIdGenerator>();
            Version = -1;
            AggregateId = _uniqueIdGenerator.NewId();
        }

        public void Load(params IDomainEvent[] history)
        {
            foreach (var e in history)
            {
                Apply(e, false);
                Version++;
            }
        }

        protected void Apply(IDomainEvent @event)
        {
            Apply(@event, true);
        }

        private void Apply(IDomainEvent @event, bool isNew)
        {
            dynamic d = this;

            d.Handle((dynamic)@event);

            if (isNew)
            {
                _domainEvents.Add(@event);
            }
        }

        protected static void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken())
            {
                throw new BusinessRuleValidationException(rule);
            }
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
    }
}
