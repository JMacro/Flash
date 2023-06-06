using Flash.Extensions.EventBus;
using Flash.Extensions.Resilience.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flash.Test.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MQTestController : ControllerBase
    {
        private readonly IEventBus _bus;
        private readonly IMonitoringApi _eventBusApi;
        private readonly IHttpClient _httpClient;
        private readonly RabbitMQOption _option;

        public MQTestController(IEventBus bus,
            IMonitoringApi eventBusApi)
        {
            this._bus = bus;
            this._eventBusApi = eventBusApi;
        }

        [HttpGet("Test1")]
        public async Task<string> Test1()
        {
            var events = new List<MessageCarrier>() {
                //MessageCarrier.Fill(new TestEvent{EventName = "routerkey.log.error"}),
                //MessageCarrier.Fill("routerkey.log.error",new TestEvent2{EventName = "routerkey.log.error"}),
                //MessageCarrier.Fill("routerkey.log.info",new TestEvent2{EventName = "routerkey.log.info"}),
            };

            for (int i = 0; i < 1000; i++)
            {
                events.Add(MessageCarrier.Fill(new TestDelayMessage { EventName = $"routerkey.log.info.{i}" }, TimeSpan.FromSeconds(30)));
            }

            var ret = await _bus.PublishAsync(events);
            return ret.ToString();
        }

        //[HttpGet("Test2")]
        //public async Task<object> Test2()
        //{
        //    return await _eventBusApi.GetQueues(new { queueName = "@Failed" });
        //}

        //[HttpGet("Test3")]
        //public async Task<object> Test3()
        //{
        //    return await _eventBusApi.GetMessages(new
        //    {
        //        vhost = "/LSXX_DEV",
        //        queueName = "TestEventHandler@Failed"
        //    });
        //}
    }
}
