using Microsoft.Extensions.DependencyInjection;

namespace Flash.Core
{
    public interface IFlashHostBuilder
    {
        IServiceCollection Services { get; }
    }
}

