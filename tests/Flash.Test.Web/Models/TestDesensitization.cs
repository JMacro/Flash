
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flash.Test.Web.Models
{
    public class TestDesensitization
    {
        public string UserName { get; set; }
        [Desensitization]
        public string Password { get; set; }
        [Desensitization]
        public TestDesensitizationSub MyProperty { get; set; }
        public int? AA { get; set; }
        [Desensitization]
        public int? BB { get; set; }
    }

    public class TestDesensitizationSub
    {
        [Desensitization]
        public string UserName { get; set; }
        [Desensitization]
        public string Password { get; set; }
    }
}
