using Flash.Core;
using Flash.Extersions.Cache.Redis;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        public static IFlashHostBuilder AddRedis(this IFlashHostBuilder hostBuilder, Action<RedisCacheConfig> action)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));

            var option = new RedisCacheConfig();
            action(option);

            if (option.HealthyCheck)
            {
                //TODO 健康检查代码实现
            }

            hostBuilder.Services.AddSingleton(CacheFactory.Build(option));
            return hostBuilder;
        }
    }
}

namespace Flash.Extersions.Cache.Redis
{

    public static class CacheFactory
    {
        public static ICacheManager Build(Action<RedisCacheConfig> action)
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