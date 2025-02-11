using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extensions.Tracting
{
    public class FlashTractingBuilder : IFlashTractingBuilder
    {
        private readonly IServiceCollection _services;

        public FlashTractingBuilder(IServiceCollection services)
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
