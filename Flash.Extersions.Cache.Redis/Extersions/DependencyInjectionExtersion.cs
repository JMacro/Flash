using Flash.Core;
using Flash.Extersions.Cache;
using Flash.Extersions.Cache.Redis;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 注册Redis缓存管理，实例对象ICacheManager
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddRedis(this IFlashHostBuilder hostBuilder, Action<ICacheConfig> action)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));

            var option = new CacheConfig();
            action(option);

            var cacheManager = CacheFactory.Build(option);

            if (option.HealthyCheck)
            {
                //TODO 健康检查代码实现
            }

            hostBuilder.Services.AddSingleton<ICacheManager>(cacheManager);
            return hostBuilder;
        }
    }
}

namespace Flash.Extersions.Cache.Redis
{
    public static class CacheFactory
    {
        public static ICacheManager Build(Action<ICacheConfig> action)
        {
            var option = new CacheConfig();
            action(option);

            var cacheManager = CacheManage.Create(option);
            return cacheManager;
        }

        public static ICacheManager Build(CacheConfig option)
        {
            var cacheManager = CacheManage.Create(option);
            return cacheManager;
        }
    }
}