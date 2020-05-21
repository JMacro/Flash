using Flash.Core;
using Flash.Extersions.Cache;
using Flash.Extersions.Cache.Redis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 注册Redis缓存管理，实例对象ICacheManager
        /// </summary>
        /// <param name="cacheBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFlashCacheBuilder AddRedis(this IFlashCacheBuilder cacheBuilder, Action<ICacheConfig> action)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));

            var option = new RedisCacheConfig();
            action(option);

            var cacheManager = CacheFactory.Build(option);

            if (option.HealthCheck)
            {
                cacheBuilder.Services.AddHealthChecks().AddRedis(ConnectionHelp.GetConnections());
            }

            if (option.DistributedLock)
            {
                cacheBuilder.Services.TryAddSingleton<IDistributedLock>(new DistributedLock(cacheManager));
            }

            cacheBuilder.Services.AddSingleton<ICacheManager>(cacheManager);
            return cacheBuilder;
        }
    }
}

namespace Flash.Extersions.Cache.Redis
{
    public static class CacheFactory
    {
        public static ICacheManager Build(Action<ICacheConfig> action)
        {
            var option = new RedisCacheConfig();
            action(option);

            var cacheManager = RedisCacheManage.Create(option);
            return cacheManager;
        }

        public static ICacheManager Build(RedisCacheConfig option)
        {
            var cacheManager = RedisCacheManage.Create(option);
            return cacheManager;
        }
    }
}