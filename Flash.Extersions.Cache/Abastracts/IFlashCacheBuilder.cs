using Flash.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extersions.Cache
{
    public interface IFlashCacheBuilder
    {
        IServiceCollection Services { get; }
        IFlashHostBuilder FlashHost { get; }
    }
}
