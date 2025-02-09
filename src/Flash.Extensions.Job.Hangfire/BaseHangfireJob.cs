using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Flash.Extensions.Job.Hangfire
{
    /// <summary>
    /// Hangfire组件抽象基类
    /// </summary>
    public abstract class BaseHangfireJob : BaseJob<PerformContext>, IRecurringJob
    {
        public ILogger _logger { get; private set; }

        protected BaseHangfireJob(ILogger logger)
        {
            this._logger = logger;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)] // 禁止并发执行，超时时间为10分钟
        public async Task Execute(PerformContext context)
        {
            try
            {
                this._logger.LogInformation($"任务【{context.BackgroundJob.Job.Type.FullName}】开始执行");
                await this.Execute(new JobExecutionContextContainer(context));
                this._logger.LogInformation($"任务【{context.BackgroundJob.Job.Type.FullName}】结束执行");
            }
            catch (System.Exception e)
            {
                this._logger.LogError(e, e.Message);
            }
        }
    }
}
