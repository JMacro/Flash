using Consul;
using Flash.Extensions.UidGenerator.Consul;
using Flash.Extensions.UidGenerator.WorkIdCreateStrategy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

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
        /// <param name="CenterId"></param>
        /// <param name="AppId"></param>
        public static void UseConsulWorkIdCreateStrategy(this IdGeneratorOption option, IConsulClient consulClient, ILogger<ConsulWorkIdCreateStrategy> logger, int CenterId, string AppId)
        {
            option.WorkIdCreateStrategy = new ConsulWorkIdCreateStrategy(consulClient, logger, CenterId, AppId, TimeSpan.FromSeconds(300), TimeSpan.FromSeconds(5));
        }
    }
}
