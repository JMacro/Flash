using Flash.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extensions.ORM
{
    public interface IFlashOrmBuilder
    {
        IServiceCollection Services { get; }
        IFlashHostBuilder FlashHost { get; }
    }
}
