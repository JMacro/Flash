using Flash.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow
{
    public class FlashWorkFlowBuilder : IFlashWorkFlowBuilder
    {
        private readonly IServiceCollection _services;
        private readonly IFlashHostBuilder _flashHost;

        public FlashWorkFlowBuilder(IServiceCollection services, IFlashHostBuilder flashHost)
        {
            this._services = services;
            this._flashHost = flashHost;
        }

        public IServiceCollection Services { get { return this._services; } }
        public IFlashHostBuilder FlashHost { get { return this._flashHost; } }
    }
}
