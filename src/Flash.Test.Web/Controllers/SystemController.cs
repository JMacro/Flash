using Flash.Test.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

        [HttpGet("Test2")]
        public object Test2()
        {
            //var data = new TestDesensitization
            //{
            //    UserName = "sdfasdfasdf",
            //    Password = "123123",
            //    MyProperty = new TestDesensitizationSub
            //    {
            //        UserName = "dfe3434"
            //    },
            //    BB = 123
            //};


            //var data = new Dictionary<string, string>();
            //data.Add("aaa", "12313");

            //var data = new List<TestDesensitization>();
            //data.Add(
            //    new TestDesensitization
            //    {
            //        UserName = "sdfasdfasdf",
            //        Password = "123123",
            //        MyProperty = new TestDesensitizationSub
            //        {
            //            UserName = "dfe3434"
            //        },
            //        BB = 123
            //    }
            //    );

            var er = JsonConvert.SerializeObject((object)data, new JsonSerializerSettings
            {
                Converters = new[] { new DesensitizationConverter() },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver(),
            });


            return data;
        }
    }
}
