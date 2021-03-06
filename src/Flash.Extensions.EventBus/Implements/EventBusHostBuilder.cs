﻿using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extensions.EventBus
{
    public class EventBusHostBuilder : IEventBusHostBuilder
    {
        private readonly IServiceCollection _services;

        public EventBusHostBuilder(IServiceCollection services)
        {
            this._services = services;
        }

        public IServiceCollection Services => this._services;
    }
}
