using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.ChangeHistory
{
    public class EntityChangeHostBuilder : IEntityChangeHostBuilder
    {
        private readonly IServiceCollection _services;

        public EntityChangeHostBuilder(IServiceCollection services)
        {
            this._services = services;
        }

        public IServiceCollection Services => this._services;
    }
}
