using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extersions.EventBus
{
    public interface IEventBusHostBuilder
    {
        IServiceCollection Services { get; }
    }
}
