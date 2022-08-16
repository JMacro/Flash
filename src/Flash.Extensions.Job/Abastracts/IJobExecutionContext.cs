namespace Flash.Extensions.Job
{
    public interface IJobExecutionContextContainer<TJobExecutionContext>
    {
        TJobExecutionContext Context { get; }
    }
}
