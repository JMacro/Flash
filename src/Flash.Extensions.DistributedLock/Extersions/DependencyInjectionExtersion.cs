using Flash.Core;
using Flash.Extensions.Cache;
using Flash.Extensions.Cache.Redis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 注册分布式锁，实例对象IDistributedLock
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddRedisDistributedLock(this IFlashHostBuilder hostBuilder, Action<RedisCacheConfig> action)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));
            var config = new RedisCacheConfig();
            action(config);

            hostBuilder.Services.TryAddSingleton<IDistributedLock>(new DistributedLock(CacheFactory.Build(config)));
            return hostBuilder;
        }
    }
}
