using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extensions.OpenTracting
{
    public interface IFlashTractingBuilder
    {
        IServiceCollection Services { get; }
    }
}
