using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Core
{
    public interface IAggregateRootHandler<TRequest>
    {
        void Handle(TRequest request);
    }
}
