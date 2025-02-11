using Flash.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extensions.ORM
{
    public interface IFlashOrmBuilder : IFlashServiceCollection
    {
        IFlashHostBuilder FlashHost { get; }
    }
}
