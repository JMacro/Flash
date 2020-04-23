using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flash.Extersions.RabbitMQ;
using Flash.Extersions.Security;
using Microsoft.AspNetCore.Mvc;

namespace Flash.Test.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly ISecurity3DES _security3DES;
        private readonly IBus _bus;
        public TestController(ISecurity3DES security3DES, IBus bus)
        {
            this._security3DES = security3DES;
            this._bus = bus;
        }

        public IActionResult Index()
        {
            string value = "1231231";
            string Encrypt = _security3DES.Encrypt(value);
            string Decrypt = _security3DES.Decrypt(Encrypt);
            var obj = new
            {
                Encrypt,
                Decrypt
            };
            return new JsonResult(obj);
        }

        public async Task<IActionResult> Send()
        {
            var message = new OrderInfo { Id = 1, OrderNumber = "123" };
            if (await _bus.PublishAsync(new List<MessageCarrier> { new MessageCarrier(typeof(OrderInfoHandler).FullName, message) }))
            {
                return Ok("OK");
            };
            return NoContent();
        }
    }

    public class OrderInfo
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
    }

    public class OrderInfoHandler : Flash.Extersions.RabbitMQ.IProcessMessageHandler<Controllers.OrderInfo>
    {
        public async Task<bool> Handle(OrderInfo message, Dictionary<string, object> headers, CancellationToken cancellationToken)
        {

            return true;
        }
    }
}