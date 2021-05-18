using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Flash.Extensions.Configuration.Json
{
    public class JsonConfigurationProvider : Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider
    {
        public JsonConfigurationProvider(Microsoft.Extensions.Configuration.Json.JsonConfigurationSource source) : base(source)
        {
        }

        public override void Load(Stream stream)
        {
            base.Data = JsonConfigurationFileParser.Parse(stream);
        }
    }
}
