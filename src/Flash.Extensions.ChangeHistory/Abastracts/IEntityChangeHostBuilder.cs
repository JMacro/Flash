using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.ChangeHistory
{
    public interface IEntityChangeHostBuilder
    {
        IServiceCollection Services { get; }
    }
}
