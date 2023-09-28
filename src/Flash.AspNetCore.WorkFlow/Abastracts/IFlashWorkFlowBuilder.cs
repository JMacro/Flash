using Flash.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.AspNetCore.WorkFlow
{
    public interface IFlashWorkFlowBuilder : IFlashServiceCollection
    {
        IFlashHostBuilder FlashHost { get; }
    }
}
