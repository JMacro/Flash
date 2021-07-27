using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extensions.Tracting
{
    public interface IFlashTractingBuilder
    {
        IServiceCollection Services { get; }
    }
}
