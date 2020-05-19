using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extersions.Cache
{
    public interface IFlashCacheBuilder
    {
        IServiceCollection Services { get; }
    }
}
