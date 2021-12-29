using Castle.DynamicProxy;
using Flash.Extensions.Cache;
using Flash.Extensions.Cache.Redis;
using Flash.Extensions.Tracting;
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

            cacheBuilder.Services.AddSingleton(sp =>
            {
                var target = CacheFactory.Build(option);
                var interceptor = sp.GetService<TracerAsyncInterceptor>();
                var generator = new ProxyGenerator();
                return generator.CreateInterfaceProxyWithTarget(target, interceptor);
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
        public static ICacheManager Build(Action<ICacheConfig> action)
        {
            var option = new RedisCacheConfig();
            action(option);
            return Build(option);
        }

        public static ICacheManager Build(RedisCacheConfig option) => RedisCacheManage.Create(option);
    }
}