using System;
using Microsoft.Extensions.Logging;

namespace Flash.Extensions.OpenTracting
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
            this._logger?.LogInformation($"{this._tracerName} {key}={value}");
        }

        public void LogException(Exception ex)
        {
            this._logger?.LogError(ex, ex.Message);
        }

        public void LogRequest(dynamic value)
        {
            this._logger?.LogInformation($"{this._tracerName} RepuestData={value}");
        }

        public void LogResponse(dynamic value)
        {
            this._logger?.LogInformation($"{this._tracerName} ResponseData={value}");
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
    }
}
