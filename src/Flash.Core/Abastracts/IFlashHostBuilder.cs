using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Core
{
    public interface IFlashHostBuilder : IFlashServiceCollection
    {
        /// <summary>
        /// Autofac容器
        /// </summary>
        ContainerBuilder Container { get; }
    }
}

