using Hangfire.Server;
using Hangfire.States;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Job.Hangfire
{
    public class HangfireJobScheduler : ICornJobScheduler
    {
        private readonly ILogger<HangfireJobScheduler> _logger;
        private readonly JobConfiguration _jobConfiguration;

        public HangfireJobScheduler(
            ILogger<HangfireJobScheduler> logger,
            JobConfiguration jobConfiguration)
        {
            this._logger = logger;
            this._jobConfiguration = jobConfiguration;
        }

        public void Dispose()
        {
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"Use {nameof(HangfireJobScheduler)}");
            if (this._jobConfiguration.Open)
            {
                try
                {
                    this._logger.LogInformation("开始初始化Job任务");

                    try
                    {
                        var recurringJobInfos = new List<RecurringJobInfo>();

                        foreach (var o in this._jobConfiguration.CronTriggers)
                        {
                            var obj = Convert(o);
                            if (obj != null) recurringJobInfos.Add(obj);
                        }

                        CronJob.AddOrUpdate(recurringJobInfos);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }

                    this._logger.LogInformation("初始化Job任务已完成");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
            return Task.CompletedTask;
        }

        public Task ShutdownAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private RecurringJobInfo Convert(CronTriggerConfiguration option)
        {
            if (!ValidateJsonOptions(option))
            {
                return null;
            }
            //var method = Type.GetType(option.JobType).GetTypeInfo().GetMethods().FirstOrDefault(p => p.ReturnType == typeof(void) && p.Name == nameof(IRecurringJob.Execute));
            var method = Type.GetType(option.JobType).GetTypeInfo().GetMethod(nameof(IRecurringJob.Execute), new Type[] { typeof(PerformContext) });

            return new RecurringJobInfo
            {
                RecurringJobId = option.JobName,
#if NET45
				Method = option.JobType.GetMethod(nameof(IRecurringJob.Execute)),
#else
                Method = method,
#endif
                Cron = option.Expression,
                Queue = option.JobGroup ?? EnqueuedState.DefaultQueue,
                TimeZone = option.TimeZone ?? TimeZoneInfo.Utc,
                JobData = option.JobData,
                Enable = option.Enable ?? true,
                IsSimpleSchedule = option.IsSimpleSchedule
            };
        }

        private bool ValidateJsonOptions(CronTriggerConfiguration option)
        {
            if (option == null) throw new ArgumentNullException(nameof(option));

            if (string.IsNullOrWhiteSpace(option.JobName))
            {
                throw new Exception($"The json token '{nameof(option.JobName)}' is null, empty, or consists only of white-space.");
            }

            if (!typeof(BaseHangfireJob).IsAssignableFrom(Type.GetType(option.JobType)))
            {
                return false;
            }
            return true;
        }
    }
}
