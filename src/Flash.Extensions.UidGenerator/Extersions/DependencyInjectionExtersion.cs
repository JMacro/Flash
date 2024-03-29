﻿using Flash.Core;
using Flash.Extensions.UidGenerator;
using Flash.Extensions.UidGenerator.WorkIdCreateStrategy;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;



namespace Microsoft.Extensions.DependencyInjection
{
    public class IdGeneratorOption
    {
        /// <summary>
        /// 服务集合
        /// </summary>
        public IServiceCollection Services { get; set; }

        /// <summary>
        /// 数据中心ID(默认0)
        /// </summary>
        public int CenterId { get; set; } = 0;

        /// <summary>
        /// 工作进程ID初始化策略
        /// </summary>
        public IWorkIdCreateStrategy WorkIdCreateStrategy { get; set; }
    }

    public static partial class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加唯一Id生成器
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddUniqueIdGenerator(this IFlashHostBuilder hostBuilder, Action<IdGeneratorOption> setup)
        {
            hostBuilder.Services.AddUniqueIdGenerator(setup);
            return hostBuilder;
        }

        /// <summary>
        /// 添加唯一Id生成器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IServiceCollection AddUniqueIdGenerator(this IServiceCollection services, Action<IdGeneratorOption> setup)
        {
            var option = new IdGeneratorOption();
            option.Services = services;
            setup(option);

            services.TryAddSingleton<IUniqueIdGenerator>(sp =>
            {
                var workId = option.WorkIdCreateStrategy.GetWorkId();
                return new SnowflakeUniqueIdGenerator(workId, option.CenterId);
            });

            if (option.WorkIdCreateStrategy != null)
            {
                services.AddSingleton<IWorkIdCreateStrategy>((sp) =>
                {
                    return option.WorkIdCreateStrategy;
                });
            }
            return services;
        }

        /// <summary>
        /// 使用静态机器标识
        /// </summary>
        /// <param name="option"></param>
        /// <param name="WorkId"></param>
        public static void UseStaticWorkIdCreateStrategy(this IdGeneratorOption option, int WorkId)
        {
            option.WorkIdCreateStrategy = new StaticWorkIdCreateStrategy(WorkId, option.CenterId);
        }
    }
}