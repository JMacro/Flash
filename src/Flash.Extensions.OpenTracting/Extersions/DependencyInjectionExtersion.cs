using Flash.Core;
using Flash.Extensions.Tracting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加日志追踪
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddLoggerTracing(this IFlashHostBuilder hostBuilder, Action<IFlashTractingBuilder> action)
        {
            var builder = new FlashTractingBuilder(hostBuilder.Services);
            action(builder);

            hostBuilder.Services.TryAddSingleton<ITracerFactory, TracerFactory>();
            return hostBuilder;
        }
    }
}
