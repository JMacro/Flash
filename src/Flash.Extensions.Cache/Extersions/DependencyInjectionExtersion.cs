using Flash.Core;
using Flash.Extensions.Cache;
using Flash.Extensions.Tracting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddCache(this IFlashHostBuilder hostBuilder, Action<IFlashCacheBuilder> action)
        {
            hostBuilder.Services.TryAdd(ServiceDescriptor.Singleton<ITracerFactory, TracerFactory>());
            hostBuilder.Services.TryAddTransient<TracerAsyncInterceptor>();
            var builder = new FlashCacheBuilder(hostBuilder.Services, hostBuilder);
            action(builder);
            return hostBuilder;
        }
    }
}
