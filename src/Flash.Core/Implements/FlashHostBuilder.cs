using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Core
{
    public class FlashHostBuilder : IFlashHostBuilder
    {
        private readonly IServiceCollection _services;
        private readonly IWebHostBuilder _builder;

        public FlashHostBuilder(IServiceCollection services)
        {
            this._services = services;
        }

        public FlashHostBuilder(IWebHostBuilder builder)
        {
            this._builder = builder;
        }

        public IServiceCollection Services => this._services;
        public IWebHostBuilder HostBuilder => _builder;
    }
}

