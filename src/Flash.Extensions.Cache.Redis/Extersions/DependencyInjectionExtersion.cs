using Flash.Core;
using Flash.Extensions.Cache;
using Flash.Extensions.Cache.Redis;
using Flash.Extensions.Tracting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
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

            cacheBuilder.Services.AddSingleton(sp =>
            {
                var tracerFactory = sp.GetService<ITracerFactory>();
                return CacheFactory.Build(option, tracerFactory);
            });

            if (option.DistributedLock)
            {
                //cacheBuilder.Services.TryAddSingleton<IDistributedLockRenewalScheduler, DistributedLockRenewalScheduler>();
                cacheBuilder.Services.TryAddSingleton((sp) => { return new DistributedLockRenewalCollection(sp); });
                cacheBuilder.Services.TryAddSingleton<IDistributedLock, DistributedLock>();
                cacheBuilder.Services.TryAddSingleton<IDistributedLockRenewalService, DistributedLockRenewalService>();
                cacheBuilder.Services.AddHostedService<DistributedLockRenewalHostedService>();
            }
            return cacheBuilder;
        }
    }
}

namespace Flash.Extensions.Cache.Redis
{
    public static class CacheFactory
    {
        public static ICacheManager Build(Action<ICacheConfig> action, ITracerFactory tracerFactory)
        {
            var option = new RedisCacheConfig();
            action(option);
            return Build(option, tracerFactory);
        }

        public static ICacheManager Build(RedisCacheConfig option, ITracerFactory tracerFactory) => RedisCacheManage.Create(option, tracerFactory);
    }
}