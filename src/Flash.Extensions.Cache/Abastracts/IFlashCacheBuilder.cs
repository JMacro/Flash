using Flash.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extensions.Cache
{
    public interface IFlashCacheBuilder
    {
        IServiceCollection Services { get; }
        IFlashHostBuilder FlashHost { get; }
    }
}
