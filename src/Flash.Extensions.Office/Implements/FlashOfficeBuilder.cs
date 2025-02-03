using Flash.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extensions.Office
{
    public class FlashOfficeBuilder : IFlashOfficeBuilder
    {
        private readonly IServiceCollection _services;
        private readonly IFlashHostBuilder _flashHost;

        public FlashOfficeBuilder(IServiceCollection services, IFlashHostBuilder flashHost)
        {
            this._services = services;
            this._flashHost = flashHost;
        }

        public IServiceCollection Services { get { return this._services; } }
        public IFlashHostBuilder FlashHost { get { return this._flashHost; } }
    }
}
