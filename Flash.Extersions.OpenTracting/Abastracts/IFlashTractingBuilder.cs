using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extersions.OpenTracting
{
    public interface IFlashTractingBuilder
    {
        IServiceCollection Services { get; }
    }
}
