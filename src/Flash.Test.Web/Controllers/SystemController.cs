using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Flash.Test.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        public SystemController()
        {
        }

        [HttpGet("Test1")]
        public object Test1()
        {
            var osVersion = System.Environment.OSVersion;
            return new
            {
                UseMemorySize = Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024,
                StartTime = Process.GetCurrentProcess().StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                WorkingSet = Process.GetCurrentProcess().WorkingSet64,
                OS = osVersion.Platform.ToString()
            };
        }
    }
}
