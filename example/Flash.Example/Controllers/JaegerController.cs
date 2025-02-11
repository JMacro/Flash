using Flash.Extensions.Tracting;
using Flash.Extensions.UidGenerator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenTracing.Util;
using System;

namespace Flash.Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JaegerController : ControllerBase
    {
        private readonly ITracerFactory _tracerFactory;
        private readonly IUniqueIdGenerator _uniqueIdGenerator;

        public JaegerController(ITracerFactory tracerFactory,IUniqueIdGenerator uniqueIdGenerator)
        {
            this._tracerFactory = tracerFactory;
            this._uniqueIdGenerator = uniqueIdGenerator;
        }

        [HttpPost("AddLogRequest")]
        public long AddLogRequest()
        {
            using (var tracer = this._tracerFactory.CreateTracer("JaegerController"))
            {
                var id = _uniqueIdGenerator.NewId();
                tracer.LogRequest(id);
                return id;
            }
        }
    }
}
