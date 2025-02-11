using Flash.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extensions.Office
{
    public interface IFlashOfficeBuilder
    {
        IServiceCollection Services { get; }
        IFlashHostBuilder FlashHost { get; }
    }
}
