using Flash.Core;
using Flash.Extensions.Office;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加办公插件
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddOffice(this IFlashHostBuilder hostBuilder, Action<IFlashOfficeBuilder> action)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));

            var builder = new FlashOfficeBuilder(hostBuilder.Services, hostBuilder);
            action(builder);

            hostBuilder.Services.AddSingleton<IOfficeSetting, OfficeSetting>(func =>
            {
                return new OfficeSetting();
            });

            return hostBuilder;
        }

        /// <summary>
        /// 添加办公插件
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="setting">初始化配置</param>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IFlashHostBuilder AddOffice(this IFlashHostBuilder hostBuilder, Action<IOfficeSetting> setting, Action<IFlashOfficeBuilder> action)
        {
            setting = setting ?? throw new ArgumentNullException(nameof(setting));
            action = action ?? throw new ArgumentNullException(nameof(action));

            var defaultSetting = new OfficeSetting();
            setting(defaultSetting);

            hostBuilder.Services.AddSingleton<IOfficeSetting, OfficeSetting>(func =>
            {
                return defaultSetting;
            });

            return AddOffice(hostBuilder, action);
        }
    }
}
