using Flash.Extensions.Job;
using Flash.Extensions.Job.Hangfire;
using Flash.Extensions.Job.Quartz;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test.Web.Jobs
{
    public class TestJob : BaseHangfireJob
    {
        public TestJob(ILogger<TestJob> logger) : base(logger)
        {
        }

        public override Task Execute(IJobExecutionContextContainer<PerformContext> contextContainer)
        {
            this._logger.LogInformation($"Run task:{nameof(TestJob)}");
            Thread.Sleep(TimeSpan.FromMinutes(2));
            return Task.CompletedTask;
        }
    }

    public class TestJob2 : BaseQuartzJob
    {
        public TestJob2(ILogger<TestJob> logger) : base(logger)
        {
        }

        public async override Task Execute(IJobExecutionContextContainer<IJobExecutionContext> contextContainer)
        {
            this._logger.LogInformation("Greetings from HelloJob!");
        }
    }

    //public class TestJob2 : IJob
    //{
    //    private readonly ILogger<TestJob> logger;

    //    public TestJob2(ILogger<TestJob> logger)
    //    {
    //        this.logger = logger;
    //    }

    //    public async Task Execute(IJobExecutionContext contextContainer)
    //    {
    //        await Console.Out.WriteLineAsync("Greetings from HelloJob!");
    //    }
    //}
}
