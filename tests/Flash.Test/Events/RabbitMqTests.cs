﻿using Autofac;
using Flash.Extensions.ChangeHistory;
using Flash.Extensions.EventBus;
using Flash.Extensions.EventBus.RabbitMQ;
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
using static Microsoft.IO.RecyclableMemoryStreamManager;

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

            eventBus.RegisterWaitAndRetry<TestEventMessage, TestEventMessageHandler>("", "");
            eventBus.Register<TestEvent2Message, TestEvent2MessageHandler>(typeof(TestEvent2MessageHandler).FullName, "routerkey.log.*");

            var result = eventBus.PublishAsync(events).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }
    }
}
