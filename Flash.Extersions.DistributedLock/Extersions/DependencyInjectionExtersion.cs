using Flash.Core;
using Flash.Extersions.Cache;
using Flash.Extersions.Cache.Redis;
using Flash.Extersions.DistributedLock;
using Flash.Extersions.DistributedLock.Implements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {

#if NETSTANDARD
        /// <summary>
        /// 注册分布式锁，实例对象IDistributedLock
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IFlashCacheBuilder AddDistributedLock(this IFlashCacheBuilder hostBuilder)
        {
            var provider = hostBuilder.Services.BuildServiceProvider();
            hostBuilder.Services.AddSingleton<IDistributedLock>(new RedisDistributedLock(provider.GetService<ICacheManager>()));
            return hostBuilder;
        }
#endif
    }
}
