using Flash.Extensions.Tracting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Flash.Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JaegerController : ControllerBase
    {
        private readonly ITracerFactory _tracerFactory;

        public JaegerController(ITracerFactory tracerFactory)
        {
            this._tracerFactory = tracerFactory;
        }

        [HttpGet("AddLogRequest")]
        public void AddLogRequest()
        {
            using (var tracer = this._tracerFactory.CreateTracer("JaegerController"))
            {
                tracer.LogRequest(Guid.NewGuid().ToString());
            }
        }
    }
}
