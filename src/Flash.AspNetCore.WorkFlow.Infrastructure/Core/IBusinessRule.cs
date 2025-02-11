using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.Core
{
    public interface IBusinessRule
    {
        bool IsBroken();

        string Message { get; }
    }
}
