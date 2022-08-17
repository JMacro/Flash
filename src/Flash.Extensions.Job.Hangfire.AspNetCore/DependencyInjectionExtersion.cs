using Flash.Core;
using Flash.Extensions.Job;
using Flash.Extensions.Job.Hangfire.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 使用Hangfire仪表盘组件
        /// </summary>
        /// <param name="app"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHangfire(this IApplicationBuilder app, Action<IGlobalJobConfiguration> setup = null)
        {
            var jobConfiguration = InitGlobalJobConfiguration(setup);
            app.UseHangfireDashboard(jobConfiguration.DashboardPath);
            return app;
        }

        /// <summary>
        /// 使用Hangfire仪表盘组件
        /// </summary>
        /// <param name="app"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHangfire(this IApplicationBuilder app, IGlobalJobConfiguration jobConfiguration)
        {
            if (jobConfiguration == null) throw new ArgumentNullException($"{nameof(jobConfiguration)} not null");
            app.UseHangfireDashboard(jobConfiguration.DashboardPath);
            return app;
        }

        /// <summary>
        /// 使用Hangfire仪表盘组件
        /// </summary>
        /// <param name="app"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IFlashApplicationBuilder UseHangfire(this IFlashApplicationBuilder flashApplication, Action<IGlobalJobConfiguration> setup = null)
        {
            var jobConfiguration = InitGlobalJobConfiguration(setup);
            flashApplication.ApplicationBuilder.UseHangfireDashboard(jobConfiguration.DashboardPath);
            return flashApplication;
        }

        /// <summary>
        /// 使用Hangfire组件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IFlashWebHostBuilder UseHangfireHostingStartup(this IFlashWebHostBuilder builder, Action<IGlobalJobConfiguration> setup)
        {
            var jobConfiguration = InitGlobalJobConfiguration(setup);
            builder.HostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IGlobalJobConfiguration>(s =>
                {
                    return jobConfiguration;
                });
            });
            builder.HostBuilder.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, typeof(HangfireHostingStartup).Namespace);
            return builder;
        }

        /// <summary>
        /// 使用Hangfire组件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IFlashWebHostBuilder UseHangfireHostingStartup(this IFlashWebHostBuilder builder)
        {
            return UseHangfireHostingStartup(builder, setup =>
            {
                setup.DashboardPath = "/hangfire";
            });
        }

        private static IGlobalJobConfiguration InitGlobalJobConfiguration(Action<IGlobalJobConfiguration> setup = null)
        {
            var configuration = new GlobalJobConfiguration();
            configuration.DashboardPath = configuration.DashboardPath ?? "/hangfire";
            setup?.Invoke(configuration);
            return configuration;
        }
    }
}
