using Flash.Extensions.EventBus;
using Flash.Extensions.Job;
using Flash.Extensions.Job.Hangfire;
using Flash.Extensions.Job.Quartz;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test.Web.Jobs
{
    public class TestJob : BaseHangfireJob
    {
        private readonly IEventBus _bus;

        public TestJob(ILogger<TestJob> logger, IEventBus bus) : base(logger)
        {
            this._bus = bus;
        }

        public override async Task Execute(IJobExecutionContextContainer<PerformContext> contextContainer)
        {
            this._logger.LogInformation($"Run task:{nameof(TestJob)}");
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Random r = new Random();
            var va = r.NextDouble() * 9 + 1;
            var events = new List<MessageCarrier>()
            {
                MessageCarrier.Fill(new TestEventMessage { EventName = $"{typeof(TestEventMessage).FullName}.{va}", Number = va }),
                MessageCarrier.Fill($"routerkey.log.error", new TestEvent2Message { EventName = $"routerkey.log.{va}",Number = va }),
                MessageCarrier.Fill($"routerkey.log.info", new TestEvent2Message { EventName = $"routerkey.log.{va}" , Number = va})
            };


            for (int i = 0; i < 10; i++)
            {
                va = r.NextDouble() * 9 + 1;
                events.Add(MessageCarrier.Fill(new TestEventMessage { EventName = $"{typeof(TestEventMessage).FullName}.{va}", Number = va }));
                events.Add(MessageCarrier.Fill($"routerkey.log.error", new TestEvent2Message { EventName = $"routerkey.log.{va}", Number = va }));
                events.Add(MessageCarrier.Fill($"routerkey.log.info", new TestEvent2Message { EventName = $"routerkey.log.{va}", Number = va }));
                events.Add(MessageCarrier.Fill(new TestDelayMessage { EventName = $"{typeof(TestDelayMessage).FullName}.{i}", Number = va }, TimeSpan.FromSeconds(va)));
            }

            var ret = await _bus.PublishAsync(events);
            //return Task.FromResult(true);
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
