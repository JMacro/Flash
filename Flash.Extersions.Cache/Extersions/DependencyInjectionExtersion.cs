using Flash.Core;
using Flash.Extersions.Cache;
using System;
using System.Collections.Generic;
using System.Text;

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
            var builder = new FlashCacheBuilder(hostBuilder.Services, hostBuilder);
            action(builder);
            return hostBuilder;
        }
    }
}
