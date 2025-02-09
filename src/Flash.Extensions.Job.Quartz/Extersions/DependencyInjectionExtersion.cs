using Flash.Core;
using Flash.Extensions.Job;
using Flash.Extensions.Job.Quartz;
using Microsoft.AspNetCore.Hosting;
using Quartz;
using Quartz.Impl;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加Quartz组件，Job类需继承<see cref="BaseQuartzJob"/>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddQuartz(this IServiceCollection services, Action<IGlobalJobConfiguration> setup = null)
        {
            services.InitGlobalJobConfiguration(setup);
            services.AddHostedService<CornJobSchedulerHostedService>();
            services.AddSingleton<ICornJobScheduler, QuartzJobScheduler>();
            services.AddQuartz(q =>
            {
                // handy when part of cluster or you want to otherwise identify multiple schedulers
                q.SchedulerId = "Scheduler-Core";

                // we take this from appsettings.json, just show it's possible
                q.SchedulerName = "Quartz ASP.NET Core Scheduler";

                // as of 3.3.2 this also injects scoped services (like EF DbContext) without problems
                q.UseMicrosoftDependencyInjectionJobFactory();
                // or for scoped service support like EF Core DbContext
                //q.UseMicrosoftDependencyInjectionScopedJobFactory();

                // these are the defaults
                q.UseSimpleTypeLoader();

                //TODO https://github.com/quartznet/quartznet/blob/main/database/tables/tables_mysql_innodb.sql
                //var config = services.BuildServiceProvider().GetService<JobConfiguration>();
                ////如果需要持久化请使用以下配置
                //if (config.IsPersistence.HasValue && config.IsPersistence.Value)
                //{
                //    if (string.IsNullOrEmpty(config.PersistenceConnection))
                //    {
                //        throw new ArgumentNullException("请配置持久化连接字符串[Hangfire:PersistenceConnection]");
                //    }

                //    q.UsePersistentStore(configure =>
                //    {
                //        configure.UseProperties = true;
                //        switch (config.PersistenceType)
                //        {
                //            case "Redis":
                //                throw new ArgumentNullException("暂不支持Redis持久化");
                //            case "MySQL":
                //                configure.UseMySql(mysql =>
                //                {
                //                    mysql.TablePrefix = "Quartz_";
                //                    mysql.ConnectionString = config.PersistenceConnection;
                //                });
                //                break;
                //            default:
                //                throw new ArgumentNullException("无法识别Hangfire持久化类型，请确认配置文件是否配置无误");
                //        }
                //    });
                //}
                //else
                {
                    q.UseInMemoryStore();
                }

                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                // also add XML configuration and poll it for changes
                q.UseXmlSchedulingConfiguration(x =>
                {
                    x.Files = new[] { "~/quartz.config" };
                    x.ScanInterval = TimeSpan.FromSeconds(2);
                    x.FailOnFileNotFound = true;
                    x.FailOnSchedulingError = true;
                });

                // convert time zones using converter that can handle Windows/Linux differences
                q.UseTimeZoneConverter();

                // auto-interrupt long-running job
                q.UseJobAutoInterrupt(options =>
                {
                    // this is the default
                    options.DefaultMaxRunTime = TimeSpan.FromMinutes(60);
                });

            })
            .AddQuartzOpenTracing()
            .AddSingleton<IScheduler>((sp) =>
            {
                var scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
                return scheduler;
            });
            return services;
        }

        /// <summary>
        /// 使用Quartz组件，Job类需继承<see cref="BaseQuartzJob"/>
        /// </summary>
        /// <param name="jobBuilder"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IFlashJobBuilder UseQuartz(this IFlashJobBuilder jobBuilder, Action<IGlobalJobConfiguration> setup = null)
        {
            jobBuilder.FlashHost.Services.AddQuartz(setup);
            return jobBuilder;
        }

        /// <summary>
        /// 使用Quartz组件，Job类需继承<see cref="BaseQuartzJob"/>
        /// </summary>
        /// <param name="jobBuilder"></param>
        /// <returns></returns>
        public static IFlashWebHostBuilder UseQuartzHostingStartup(this IFlashWebHostBuilder builder, Action<IGlobalJobConfiguration> setup)
        {
            builder.HostBuilder.ConfigureServices(services =>
            {
                InitGlobalJobConfiguration(services, setup);
            });

            builder.HostBuilder.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, typeof(QuartzHostingStartup).Namespace);
            return builder;
        }

        /// <summary>
        /// 使用Quartz组件，Job类需继承<see cref="BaseQuartzJob"/>
        /// </summary>
        /// <param name="jobBuilder"></param>
        /// <returns></returns>
        public static IFlashWebHostBuilder UseQuartzHostingStartup(this IFlashWebHostBuilder builder)
        {
            return UseQuartzHostingStartup(builder, setup =>
            {
                setup.DashboardPath = "/quartz";
            });
        }

        private static IGlobalJobConfiguration InitGlobalJobConfiguration(this IServiceCollection services, Action<IGlobalJobConfiguration> setup = null)
        {
            var configuration = new GlobalJobConfiguration();
            configuration.DashboardPath = configuration.DashboardPath ?? "/quartz";
            services.AddSingleton<IGlobalJobConfiguration>(s =>
            {
                return configuration;
            });
            setup?.Invoke(configuration);
            return configuration;
        }
    }
}
