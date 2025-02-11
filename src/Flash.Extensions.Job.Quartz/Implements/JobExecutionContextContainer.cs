using Quartz;

namespace Flash.Extensions.Job.Quartz
{
    internal class JobExecutionContextContainer : IJobExecutionContextContainer<IJobExecutionContext>
    {
        public JobExecutionContextContainer(IJobExecutionContext context)
        {
            Context = context;
        }

        public IJobExecutionContext Context { get; internal set; }
    }
}
