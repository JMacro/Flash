using MediatR;
using System;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.Core
{
    /// <summary>
    /// 领域事件
    /// </summary>
    public interface IDomainEvent : INotification, IRequest
    {
        /// <summary>
        /// 事件Id
        /// </summary>
        long Id { get; set; }
        /// <summary>
        /// 聚合根Id
        /// </summary>
        long AggregateId { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        DateTime OccurredOn { get; }
        /// <summary>
        /// 版本
        /// </summary>
        int Version { get; set; }
    }
}
