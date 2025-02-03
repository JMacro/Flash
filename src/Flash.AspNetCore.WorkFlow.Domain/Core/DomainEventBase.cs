using Flash.AspNetCore.WorkFlow.Infrastructure.Core;
using System;

namespace Flash.AspNetCore.WorkFlow.Domain.Core
{
    /// <summary>
    /// 领域事件基类
    /// </summary>
    public class DomainEventBase : IDomainEvent
    {
        public long Id { get; set; }
        public long AggregateId { get; set; }
        public DateTime OccurredOn => DateTime.Now;
        public int Version { get; set; }
    }
}
