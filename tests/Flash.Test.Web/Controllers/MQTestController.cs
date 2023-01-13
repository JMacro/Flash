﻿using Flash.Extensions.EventBus;
using Flash.Extensions.Resilience.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
            var item1 = new MessageCarrier("routerkey.log.error", new TestEvent
            {
                EventName = "routerkey.log.error"
            });

            var item2 = new MessageCarrier("routerkey.log.info", new TestEvent
            {
                EventName = "routerkey.log.info"
            });


            var events = new List<MessageCarrier>() {
                   item1,item2
            };

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