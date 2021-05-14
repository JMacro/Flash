using Flash.Extersions.EventBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flash.Test.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MQTestController : ControllerBase
    {
        private readonly IEventBus _bus;

        public MQTestController(IEventBus bus)
        {
            this._bus = bus;
        }

        [HttpGet("Test1")]
        public async Task<string> Test1()
        {
            var item1 = new MessageCarrier("TestEventHandler", new TestEvent
            {
                EventName = "Test1"
            });

            var item2 = new MessageCarrier("TestEventHandler2", new TestEvent
            {
                EventName = "Test2"
            });


            var events = new List<MessageCarrier>() {
                   item1,item2
            };

            var ret = await _bus.PublishAsync(events);
            return ret.ToString();
        }
    }
}
