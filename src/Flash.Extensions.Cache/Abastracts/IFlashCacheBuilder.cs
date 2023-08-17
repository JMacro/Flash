using Flash.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extensions.Cache
{
    public interface IFlashCacheBuilder : IFlashServiceCollection
    {
        IFlashHostBuilder FlashHost { get; }
    }
}
