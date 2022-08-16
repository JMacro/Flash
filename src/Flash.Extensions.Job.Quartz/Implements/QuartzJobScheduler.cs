using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Job.Quartz
{
    public class QuartzJobScheduler : ICornJobScheduler
    {
        private readonly ILogger<QuartzJobScheduler> _logger;
        private readonly IScheduler _scheduler;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IJobFactory _jobFactory;
        private readonly JobConfiguration _jobConfiguration;

        public QuartzJobScheduler(
            ILogger<QuartzJobScheduler> logger,
            IScheduler scheduler,
            ISchedulerFactory schedulerFactory,
            IServiceProvider serviceProvider,
            IJobFactory jobFactory,
            JobConfiguration jobConfiguration)
        {
            this._logger = logger;
            this._scheduler = scheduler;
            this._schedulerFactory = schedulerFactory;
            this._serviceProvider = serviceProvider;
            this._jobFactory = jobFactory;
            this._jobConfiguration = jobConfiguration;
        }

        public async void Dispose()
        {
            if (this._scheduler != null)
            {
                await this._scheduler.Shutdown(true);
            }
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"Use {nameof(QuartzJobScheduler)}");
            if (this._jobConfiguration.Open)
            {
                try
                {
                    this._logger.LogInformation("开始初始化Job任务");
                    for (var i = 0; i < this._jobConfiguration.CronTriggers.Count; i++)
                    {
                        try
                        {
                            var triggerConfiguration = this._jobConfiguration.CronTriggers[i];

                            if (!triggerConfiguration.Enable.HasValue || triggerConfiguration.Enable.Value)
                            {
                                this._logger.LogInformation($"正在初始化Job任务：{triggerConfiguration.JobName}");

                                var jobType = Type.GetType(triggerConfiguration.JobType);
                                if (jobType == null || !typeof(BaseQuartzJob).IsAssignableFrom(jobType))
                                {
                                    _logger.LogWarning("Quartz create job " + triggerConfiguration.JobType + " error");
                                    continue;
                                }

                                var jobDataMap = new JobDataMap();
                                if (triggerConfiguration.JobData != null)
                                {
                                    foreach (var item in triggerConfiguration.JobData)
                                    {
                                        jobDataMap.Add(item.Key, item.Value);
                                    }
                                }

                                var job = JobBuilder.Create(jobType)
                                    .WithIdentity(triggerConfiguration.JobName, triggerConfiguration.JobGroup)
                                    .SetJobData(jobDataMap).Build();    //创建一个任务        

                                ITrigger cronTrigger = null;
                                if (triggerConfiguration.IsSimpleSchedule)
                                {
                                    cronTrigger = TriggerBuilder.Create()
                                    .WithIdentity(triggerConfiguration.JobName, triggerConfiguration.JobGroup)
                                    .StartNow()
                                    .WithSimpleSchedule(a =>
                                    {
                                        a.WithRepeatCount(0);
                                    })
                                    .Build();
                                }
                                else
                                {
                                    cronTrigger = TriggerBuilder.Create()
                                    .WithIdentity(triggerConfiguration.JobName, triggerConfiguration.JobGroup)
                                    .StartNow()
                                    .WithCronSchedule(triggerConfiguration.Expression)
                                    .Build();
                                }

                                this._scheduler.JobFactory = this._jobFactory;
                                await this._scheduler.ScheduleJob(job, cronTrigger, cancellationToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                        }
                    }
                    await this._scheduler.Start();
                    this._logger.LogInformation("初始化Job任务已完成");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }

        public async Task ShutdownAsync(CancellationToken cancellationToken)
        {
            if (!this._scheduler.IsShutdown)
            {
                await this._scheduler.Shutdown(cancellationToken);
            }
        }
    }
}
