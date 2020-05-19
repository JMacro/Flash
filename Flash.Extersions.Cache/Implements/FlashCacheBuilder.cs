using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extersions.Cache
{
    public class FlashCacheBuilder : IFlashCacheBuilder
    {
        private readonly IServiceCollection _services;

        public FlashCacheBuilder(IServiceCollection services)
        {
            this._services = services;
        }

        public IServiceCollection Services
        {
            get
            {
                return this._services;
            }
        }
    }
}
