using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extersions.OpenTracting
{
    public class LinkTrack : ILinkTrack
    {
        private readonly OpenTracing.IScope Scope;

        public LinkTrack(string operationName)
        {
            Scope = OpenTracing.Util.GlobalTracer.Instance.BuildSpan(operationName).StartActive();
        }

        public void Dispose()
        {
            Scope.Span.Finish();
            Scope.Dispose();
        }

        public void Log(string key, dynamic value)
        {
            if (value is string)
            {
                var dic = new Dictionary<string, object>
                {
                    [key] = value
                };
                Scope.Span.Log(dic);
            }
            else
            {
                var dic = new Dictionary<string, object>
                {
                    //TODO 数据脱敏处理
                    [key] = JsonConvert.SerializeObject(value) as object
                };
                Scope.Span.Log(dic);
            }
        }

        public void LogException(Exception ex)
        {
            var filed = new Dictionary<string, object>
            {
                ["stack"] = ex.StackTrace,
                ["error.kind"] = ex.Message,
                ["error.object"] = ex
            };
            Scope.Span.Log(filed);
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

        public void SetError()
        {
            SetTag("error", true);
        }

        public void SetTag(string key, dynamic value)
        {
            Scope.Span.SetTag(key, value);
        }
    }
}
