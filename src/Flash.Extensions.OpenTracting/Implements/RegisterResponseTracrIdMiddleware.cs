using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Flash.Extensions.OpenTracting
{
    public class RegisterResponseTracrIdMiddleware
	{
        private readonly RequestDelegate _next;
        private readonly IRegisterResponseTracrIdService _service;

        public RegisterResponseTracrIdMiddleware()
		{
		}

        public RegisterResponseTracrIdMiddleware(RequestDelegate next, IRegisterResponseTracrIdService service)
        {
            this._next = next;
            this._service = service;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("Trace-Id", this._service.GetTraceId());
            await this._next.Invoke(context);
        }
    }
}

