using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extensions.EventBus
{
    public interface IEventBusHostBuilder
    {
        IServiceCollection Services { get; }
    }
}
