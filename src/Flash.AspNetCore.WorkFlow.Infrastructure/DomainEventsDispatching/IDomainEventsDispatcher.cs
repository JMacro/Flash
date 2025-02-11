using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.DomainEventsDispatching
{
    /// <summary>
    /// 领域事件分发器接口
    /// </summary>
    public interface IDomainEventsDispatcher
    {
        Task DispatchEventsAsync();
    }
}
