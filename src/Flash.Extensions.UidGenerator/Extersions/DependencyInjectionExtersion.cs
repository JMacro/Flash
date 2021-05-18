using Flash.Core;
using Flash.Extensions.UidGenerator;
using Flash.Extensions.UidGenerator.WorkIdCreateStrategy;
using System;



namespace Microsoft.Extensions.DependencyInjection
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IdGeneratorOption”的 XML 注释
    public class IdGeneratorOption
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IdGeneratorOption”的 XML 注释
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


#pragma warning disable CS1591 // 缺少对公共可见类型或成员“DependencyInjectionExtersion”的 XML 注释
    public static partial class DependencyInjectionExtersion
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“DependencyInjectionExtersion”的 XML 注释
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“DependencyInjectionExtersion.AddUniqueIdGenerator(IFlashHostBuilder, Action<IdGeneratorOption>)”的 XML 注释
        public static IFlashHostBuilder AddUniqueIdGenerator(this IFlashHostBuilder hostBuilder, Action<IdGeneratorOption> setup)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“DependencyInjectionExtersion.AddUniqueIdGenerator(IFlashHostBuilder, Action<IdGeneratorOption>)”的 XML 注释
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

#pragma warning disable CS1591 // 缺少对公共可见类型或成员“DependencyInjectionExtersion.UseStaticWorkIdCreateStrategy(IdGeneratorOption, int)”的 XML 注释
        public static void UseStaticWorkIdCreateStrategy(this IdGeneratorOption option, int WorkId)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“DependencyInjectionExtersion.UseStaticWorkIdCreateStrategy(IdGeneratorOption, int)”的 XML 注释
        {
            option.WorkIdCreateStrategy = new StaticWorkIdCreateStrategy(WorkId);
        }
    }

}
