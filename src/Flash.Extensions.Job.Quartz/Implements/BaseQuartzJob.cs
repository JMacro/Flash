using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace Flash.Extensions.Job.Quartz
{
    /// <summary>
    /// Quartz组件抽象基类
    /// </summary>
    public abstract class BaseQuartzJob : BaseJob<IJobExecutionContext>, IJob
    {
        public ILogger _logger { get; private set; }

        protected BaseQuartzJob(ILogger logger)
        {
            this._logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                this._logger.LogInformation($"任务【{context.JobDetail.Key.Name}】开始执行");
                await this.Execute(new JobExecutionContextContainer(context));
                this._logger.LogInformation($"任务【{context.JobDetail.Key.Name}】结束执行");
            }
            catch (System.Exception e)
            {
                this._logger.LogError(e, e.Message);
            }
        }
    }
}
