using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Flash.Extensions.Tracting
{
    public class DefaultTracer : ITracer
    {
        private readonly ILogger<DefaultTracer> _logger;
        private string _tracerName;

        public DefaultTracer(ILogger<DefaultTracer> logger)
        {
            this._logger = logger;
        }

        public void Dispose()
        {
        }

        public void Log(string key, dynamic value)
        {
            if (value is string)
            {
                this._logger?.LogInformation($"{this._tracerName} {key}={value}");
            }
            else
            {
                this._logger?.LogInformation($"{this._tracerName} {key}={SerializeObject(value)}");
            }
        }

        public void LogException(Exception ex)
        {
            this._logger?.LogError(ex, ex.Message);
        }

        public void LogRequest(dynamic value)
        {
            Log("request", value);
        }

        public void LogResponse(dynamic value)
        {
            Log("response", value);
        }

        public void SetComponent(string componentName)
        {

        }

        public void SetError()
        {
            this._logger?.LogError("error=true");
        }

        public void SetTag(string key, dynamic value)
        {
            this._logger?.LogInformation($"{this._tracerName} {key}={value}");
        }

        public void SetTracerName(string tracerName)
        {
            this._tracerName = tracerName;
        }

        private string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                Converters = new[] { new DesensitizationConverter() },
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            });
        }
    }
}
