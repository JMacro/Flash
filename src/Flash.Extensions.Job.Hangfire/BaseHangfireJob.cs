using Hangfire.Server;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Flash.Extensions.Job.Hangfire
{
    /// <summary>
    /// Quartz组件抽象基类
    /// </summary>
    public abstract class BaseHangfireJob : BaseJob<PerformContext>, IRecurringJob
    {
        public ILogger _logger { get; private set; }

        protected BaseHangfireJob(ILogger logger)
        {
            this._logger = logger;
        }

        public void Execute(PerformContext context)
        {
            try
            {
                this._logger.LogInformation($"任务【{context.BackgroundJob.Job.Type.FullName}】开始执行");
                this.Execute(new JobExecutionContextContainer(context)).ConfigureAwait(false).GetAwaiter().GetResult();
                this._logger.LogInformation($"任务【{context.BackgroundJob.Job.Type.FullName}】结束执行");
            }
            catch (System.Exception e)
            {
                this._logger.LogError(e, e.Message);
            }
        }
    }
}
