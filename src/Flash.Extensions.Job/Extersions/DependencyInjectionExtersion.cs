using Flash.Core;
using Flash.Extensions.Job;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加Job
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddJob(this IFlashHostBuilder hostBuilder, Action<IFlashJobBuilder> action, string configName = "CornJobScheduler")
        {
            hostBuilder.Services.AddJobConfiguration(configName);
            var builder = new FlashJobBuilder(hostBuilder);
            action(builder);
            return hostBuilder;
        }

        /// <summary>
        /// 添加Job配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static IServiceCollection AddJobConfiguration(this IServiceCollection services, string configName = "CornJobScheduler")
        {
            services.AddSingleton((s) =>
            {
                var configuration = s.GetService<IConfiguration>();
                var logger = s.GetService<ILogger<IConfiguration>>();
                var config = configuration.GetSection(configName).Get<JobConfiguration>();
                if (config == null)
                {
                    logger.LogWarning($"configuration section '{configName}:CornJobScheduler' not found");
                    config = new JobConfiguration();
                }
                return config;
            });

            return services;
        }
    }
}
