using Microsoft.Extensions.DependencyInjection;

namespace Flash.Core
{
    public class FlashHostBuilder : IFlashHostBuilder
    {
        private readonly IServiceCollection _services;

        public FlashHostBuilder(IServiceCollection Services)
        {
            this._services = Services;
        }

        public IServiceCollection Services
        {
            get
            {
                return _services;
            }
        }
    }
}

