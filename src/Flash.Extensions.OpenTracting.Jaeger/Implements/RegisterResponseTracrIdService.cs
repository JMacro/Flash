using OpenTracing.Util;

namespace Flash.Extensions.OpenTracting.Jaeger
{
    public class RegisterResponseTracrIdService: IRegisterResponseTracrIdService
    {
		public RegisterResponseTracrIdService()
		{
		}

        public string GetTraceId()
        {
            return GlobalTracer.Instance.ScopeManager.Active.Span.Context.SpanId;
        }
    }
}

