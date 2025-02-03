using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Core
{
    public class FlashHostBuilder : IFlashHostBuilder
    {
        private readonly IServiceCollection _services;
        private readonly IWebHostBuilder _builder;
        private readonly ContainerBuilder _container;

        public FlashHostBuilder(IServiceCollection services, ContainerBuilder container = null)
        {
            this._services = services;
            this._container = container ?? new ContainerBuilder();
        }

        public FlashHostBuilder(IWebHostBuilder builder)
        {
            this._builder = builder;
        }

        public IServiceCollection Services => this._services;
        public IWebHostBuilder HostBuilder => this._builder;

        public ContainerBuilder Container => this._container;
    }
}

