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
            hostBuilder.Services.AddSingleton((s) =>
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

            var builder = new FlashJobBuilder(hostBuilder);
            action(builder);
            return hostBuilder;
        }
    }
}
