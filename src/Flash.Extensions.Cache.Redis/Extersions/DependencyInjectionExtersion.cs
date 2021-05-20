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
        /// 注册Redis缓存管理，实例对象ICacheManager
        /// </summary>
        /// <param name="cacheBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFlashCacheBuilder UseRedis(this IFlashCacheBuilder cacheBuilder, Action<ICacheConfig> action)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));

            var option = new RedisCacheConfig();
            action(option);

            var cacheManager = CacheFactory.Build(option);
            cacheBuilder.Services.AddSingleton<ICacheManager>(cacheManager);

            if (option.DistributedLock)
            {
                cacheBuilder.Services.TryAddSingleton<IDistributedLock>(new DistributedLock(cacheManager));
            }
            return cacheBuilder;
        }
    }
}

namespace Flash.Extensions.Cache.Redis
{
    public static class CacheFactory
    {
        public static ICacheManager Build(Action<ICacheConfig> action)
        {
            var option = new RedisCacheConfig();
            action(option);
            return Build(option);
        }

        public static ICacheManager Build(RedisCacheConfig option) => RedisCacheManage.Create(option);
    }
}