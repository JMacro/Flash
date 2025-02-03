using Flash.Extensions.Tracting;
using Flash.Extensions.Tracting.Skywalking;
using Flash.Extensions.Tracting.Skywalking.Segments;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Flash.Extensions.OpenTracting.Skywalking
{
    public class SkywalkingTracer : ITracer
    {
        private SegmentContext SegmentContext;
        private static object _async = new object();
        private readonly ITracingContext _tracingContext;

        public SkywalkingTracer(ITracingContext tracingContext)
        {
            this._tracingContext = tracingContext;
        }

        public void Dispose()
        {
        }

        public void Log(string key, dynamic value)
        {
            if (value is string)
            {
                var dic = new Dictionary<string, object>
                {
                    [key] = value
                };
                this.SegmentContext.Span.AddLog(new LogEvent(key, value));
            }
            else
            {
                var dic = new Dictionary<string, object>
                {
                    [key] = SerializeObject(value) as object
                };
                this.SegmentContext.Span.AddLog(new LogEvent(key, SerializeObject(value)));
            }
        }

        public void LogException(Exception ex)
        {
            var filed = new List<LogEvent> {
                new LogEvent("stack", ex.StackTrace),
                new LogEvent("error.kind", ex.Message),
                new LogEvent("error.object", SerializeObject(ex)),
            };
            this.SegmentContext.Span.AddLog(filed.ToArray());
            SetError();
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
            SetTag("component", componentName);
        }

        public void SetError()
        {
            SetTag("error", true);
        }

        public void SetTag(string key, dynamic value)
        {
            this.SegmentContext.Span.AddTag(key, value);
        }

        public void SetTracerName(string tracerName)
        {
            if (this.SegmentContext == null)
            {
                lock (_async)
                {
                    if (this.SegmentContext == null)
                    {
                        this.SegmentContext = this._tracingContext.CreateLocalSegmentContext(tracerName);
                    }
                }
            }
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
