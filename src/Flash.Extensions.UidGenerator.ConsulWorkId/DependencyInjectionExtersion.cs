using Consul;
using Flash.Extensions;
using Flash.Extensions.UidGenerator.ConsulWorkId;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
        /// <summary>
        /// 使用Consul机器标识
        /// </summary>
        /// <param name="option"></param>
        /// <param name="consulClient"></param>
        /// <param name="logger"></param>
        /// <param name="centerId"></param>
        /// <param name="AppId"></param>
        public static void UseConsulWorkIdCreateStrategy(this IdGeneratorOption option, IConsulClient consulClient, ILogger<ConsulWorkIdCreateStrategy> logger, int centerId, string appId)
        {
            Check.Argument.IsNotEmpty(appId, "appsettings.json => FlashConfiguration:UniqueIdGenerator:AppId");

            option.WorkIdCreateStrategy = new ConsulWorkIdCreateStrategy(consulClient, logger, centerId, appId, TimeSpan.FromSeconds(300), TimeSpan.FromSeconds(5));
        }
    }
}
