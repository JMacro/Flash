using Flash.Extensions.Job;
using Flash.Extensions.Job.Hangfire;
using Hangfire;
using Hangfire.Console;
using Hangfire.MemoryStorage;
using Hangfire.MySql;
using Hangfire.Redis;
using Microsoft.Extensions.Logging;
using System;
using System.Transactions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加Hangfire组件
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddHangfire(this IServiceCollection services)
        {
            services.AddHangfire(opts =>
            {
                var config = services.BuildServiceProvider().GetService<JobConfiguration>();
                //如果需要持久化请使用以下配置
                if (config.IsPersistence.HasValue && config.IsPersistence.Value)
                {
                    if (string.IsNullOrEmpty(config.PersistenceConnection))
                    {
                        throw new ArgumentNullException("请配置持久化连接字符串[Hangfire:PersistenceConnection]");
                    }
                                        
                    switch (config.PersistenceType)
                    {
                        case "Redis":
                            if (string.IsNullOrEmpty(config.PersistencePrefix)) config.PersistencePrefix = "Hangfire:";
                            opts.UseRedisStorage(config.PersistenceConnection, new RedisStorageOptions
                            {
                                Prefix = config.PersistencePrefix,
                            });
                            break;
                        case "MySQL":
                            if (string.IsNullOrEmpty(config.PersistencePrefix)) config.PersistencePrefix = "Hangfire";
                            opts.UseStorage(new MySqlStorage(config.PersistenceConnection,
                                new MySqlStorageOptions
                                {
                                    TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                                    QueuePollInterval = TimeSpan.FromSeconds(15),
                                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                                    PrepareSchemaIfNecessary = true,
                                    DashboardJobListLimit = 50000,
                                    TransactionTimeout = TimeSpan.FromMinutes(1),
                                    TablesPrefix = config.PersistencePrefix,
                                }));
                            break;
                        default:
                            throw new ArgumentNullException("无法识别Hangfire持久化类型，请确认配置文件是否配置无误");
                    }
                }
                else
                {
                    opts.UseMemoryStorage();
                }
                opts.UseLog4NetLogProvider();
                opts.UseConsole();
            });

            services.AddHangfireServer(options =>
            {
                options.ServerName = String.Format("{0}.{1}", Environment.MachineName, Guid.NewGuid().ToString()); //服务器唯一的标识符
            });

            services.AddSingleton<ICornJobScheduler>((c) =>
            {
                var jobStorage = c.GetService<JobStorage>();
                return new HangfireJobScheduler(c.GetService<ILogger<HangfireJobScheduler>>(), c.GetService<JobConfiguration>());
            });

            services.AddHostedService<CornJobSchedulerHostedService>();

            return services;
        }

        /// <summary>
        /// 使用Hangfire组件
        /// </summary>
        /// <param name="jobBuilder"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IFlashJobBuilder UseHangfire(this IFlashJobBuilder jobBuilder)
        {
            jobBuilder.FlashHost.Services.AddHangfire();
            return jobBuilder;
        }
    }
}
