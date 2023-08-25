using Flash.Extensions.CompareObjects;
using Microsoft.Extensions.DependencyInjection;

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

        public ComparisonConfig Config { get; private set; }
    }
}
