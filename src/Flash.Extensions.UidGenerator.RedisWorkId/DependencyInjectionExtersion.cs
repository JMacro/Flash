using Flash.Extensions.Cache;
using Flash.Extensions.UidGenerator.RedisWorkId;
using Flash.Extensions.UidGenerator.WorkIdCreateStrategy;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
        /// <summary>
        /// 使用Redis机器标识
        /// </summary>
        /// <param name="option"></param>
        /// <param name="cacheClient"></param>
        /// <param name="logger"></param>
        /// <param name="centerId"></param>
        /// <param name="appId"></param>
        public static void UseRedisWorkIdCreateStrategy(this IdGeneratorOption option, ICacheManager cacheClient, ILogger<RedisWorkIdCreateStrategy> logger, int centerId, string appId)
        {
            Flash.Extensions.Check.Argument.IsNotEmpty(appId, "appsettings.json => FlashConfiguration:UniqueIdGenerator:AppId");

            option.WorkIdCreateStrategy = new RedisWorkIdCreateStrategy(cacheClient, logger, centerId, appId, TimeSpan.FromSeconds(300));
            option.Services.AddHostedService(sp =>
            {
                return new RedisWorkIdRenewalHostedService(
                    cacheClient,
                    sp.GetService<IHostApplicationLifetime>(),
                    sp.GetService<IWorkIdCreateStrategy>(),
                    sp.GetService<ILogger<RedisWorkIdRenewalHostedService>>(),
                    appId,
                    TimeSpan.FromSeconds(150));
            });
        }
    }
}
