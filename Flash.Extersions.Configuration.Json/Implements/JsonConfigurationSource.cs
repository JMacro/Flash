using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extersions.Configuration.Json
{
    public class JsonConfigurationSource : Microsoft.Extensions.Configuration.Json.JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new JsonConfigurationProvider(this);
        }
    }
}
