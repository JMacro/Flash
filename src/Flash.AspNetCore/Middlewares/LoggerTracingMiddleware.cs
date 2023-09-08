using Flash.Extensions.Tracting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Flash.AspNetCore.Middlewares
{
    public class LoggerTracingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ITracerFactory _tracerFactory;

        public LoggerTracingMiddleware(RequestDelegate next, IConfiguration configuration, ITracerFactory tracerFactory)
        {
            this._next = next;
            this._configuration = configuration;
            this._tracerFactory = tracerFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            using (var tracer = this._tracerFactory.CreateTracer(context.Request.Path.Value))
            {
                var originalResponseStream = context.Response.Body;

                var requestMethod = context.Request.Method.ToUpper();
                tracer.SetTag("component", nameof(LoggerTracingMiddleware));
                tracer.SetTag("http.method", requestMethod);

                if (requestMethod == HttpMethod.Get.Method)
                {
                    tracer.LogRequest(context.Request.QueryString.Value);
                }

                if (requestMethod == HttpMethod.Post.Method)
                {
                    context.Request.EnableBuffering();
                    var stream = new StreamReader(context.Request.Body);
                    var requestBody = await stream.ReadToEndAsync();
                    tracer.LogRequest(requestBody);
                }

                using (var swapStream = new MemoryStream())
                {
                    context.Response.Body = swapStream;
                    await this._next.Invoke(context);

                    swapStream.Position = 0;

                    var stream = new StreamReader(swapStream);
                    var responseBody = await stream.ReadToEndAsync();
                    tracer.LogResponse(responseBody);

                    swapStream.Position = 0;
                    await swapStream.CopyToAsync(originalResponseStream);
                    context.Response.Body = originalResponseStream;
                }
            }
        }
    }
}