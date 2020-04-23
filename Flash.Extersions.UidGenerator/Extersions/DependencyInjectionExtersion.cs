﻿using Flash.Core;
using Flash.Extersions.UidGenerator;
using Flash.Extersions.UidGenerator.WorkIdCreateStrategy;
using System;



namespace Microsoft.Extensions.DependencyInjection
{
    public class IdGeneratorOption
    {

        /// <summary>
        /// 数据中心ID(默认0)
        /// </summary>
        public int CenterId { get; set; } = 0;

        /// <summary>
        /// 工作进程ID初始化策略
        /// </summary>
        internal IWorkIdCreateStrategy WorkIdCreateStrategy { get; set; }
    }


    public static partial class DependencyInjectionExtersion
    {
        public static IFlashHostBuilder AddUniqueIdGenerator(this IFlashHostBuilder hostBuilder, Action<IdGeneratorOption> setup)
        {
            var option = new IdGeneratorOption();
            setup(option);

            hostBuilder.Services.AddSingleton<IUniqueIdGenerator>(sp =>
            {
                var workId = option.WorkIdCreateStrategy.NextId();
                return new SnowflakeUniqueIdGenerator(workId, option.CenterId);
            });
            return hostBuilder;
        }

        public static void UseStaticWorkIdCreateStrategy(this IdGeneratorOption option, int WorkId)
        {
            option.WorkIdCreateStrategy = new StaticWorkIdCreateStrategy(WorkId);
        }
    }

}
