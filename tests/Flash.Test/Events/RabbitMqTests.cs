using Autofac;
using Flash.Extensions.ChangeHistory;
using Flash.Extensions.EventBus;
using Flash.Test.Events.Messages;
using Flash.Test.StartupTests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Test.Events
{
    [TestFixture]
    public class RabbitMqTests
    {
        [Test]
        public void PublishTest()
        {
            var webHostBuilder = new WebHostBuilder()
             .UseStartup<RabbitMqStartupTest>();

            var server = new TestServer(webHostBuilder);
            var eventBus = server.Services.GetService<IEventBus>();

            var events = new List<MessageCarrier>() {
                MessageCarrier.Fill("routerkey.log.error",new TestEvent2Message{EventName = "routerkey.log.error"})
            };

            var result = eventBus.PublishAsync(events).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }
    }
}
