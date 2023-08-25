using Flash.Core;
using Flash.Extensions.ChangeHistory;
using Flash.Extensions.CompareObjects;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加实体变更日志记录
        /// <para><see cref="IEntityChange"/></para>
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddEntityChange(this IFlashHostBuilder hostBuilder, Action<IEntityChangeHostBuilder> setup)
        {
            var builder = new EntityChangeHostBuilder(hostBuilder.Services);
            setup(builder);

            hostBuilder.Services.TryAddSingleton<ICompareLogic>((sp) =>
            {
                var config = sp.GetService<ComparisonConfig>();
                if (config == null) throw new ArgumentNullException($"请初始化对比器配置信息。【初始化方式：{nameof(IEntityChangeHostBuilder)}.{nameof(InitConfig)}({nameof(Action)}<{nameof(ComparisonConfig)}> setup)】");
                var compareLogic = new CompareLogic(config);
                return compareLogic;
            });
            hostBuilder.Services.TryAddSingleton<IEntityChange>((sp) =>
            {
                var logger = sp.GetService<ILogger<EntityChange>>();
                var compareLogic = sp.GetService<ICompareLogic>();
                var storage = sp.GetService<IStorage>();
                if (storage == null)
                {
                    storage = new DefaultStorage(sp.GetService<ILogger<DefaultStorage>>());
                }
                return new EntityChange(storage, compareLogic);
            });

            return hostBuilder;
        }

        /// <summary>
        /// 初始化对比器配置信息
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IEntityChangeHostBuilder InitConfig(this IEntityChangeHostBuilder hostBuilder, Action<ComparisonConfig> setup)
        {
            var config = new ComparisonConfig();
            setup(config);
            hostBuilder.Services.TryAddSingleton<ComparisonConfig>((sp) =>
            {
                return config;
            });
            return hostBuilder;
        }
    }
}
