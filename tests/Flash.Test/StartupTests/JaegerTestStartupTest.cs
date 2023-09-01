

using Flash.Extensions.Tracting;
using Flash.Test.OpenTracting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flash.Test.StartupTests
{
    public class JaegerTestStartupTest : BaseStartupTest
    {
        public JaegerTestStartupTest(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });
            services.AddFlash(flash =>
            {
                flash.AddLoggerTracing(option =>
                {
                    option.UseJaeger(config =>
                    {
                        config.AgentHost = "";
                        config.AgentPort = 5775;
                        config.SerivceName = this.GetType().FullName;
                        config.EndPoint = "http://192.168.50.242:14268/api/traces";
                        config.Open = true;
                    });
                });
            });
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.Map("/jaeger", builder =>
            {
                builder.Use(async (HttpContext context, RequestDelegate next) =>
                {
                    var tracerFactory = app.ApplicationServices.GetService<ITracerFactory>();
                    using (var tracer = tracerFactory.CreateTracer($"{nameof(JaegerTests)}"))
                    {
                        tracer.LogRequest("123");
                    }
                    await context.Response.WriteAsync("OK");
                });
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
    }
}
