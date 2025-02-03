using Hangfire.Server;

namespace Flash.Extensions.Job.Hangfire
{
    internal class JobExecutionContextContainer : IJobExecutionContextContainer<PerformContext>
    {
        public JobExecutionContextContainer(PerformContext context)
        {
            Context = context;
        }

        public PerformContext Context { get; internal set; }
    }
}
