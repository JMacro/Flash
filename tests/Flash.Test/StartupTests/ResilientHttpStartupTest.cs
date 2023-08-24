using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Test.StartupTests
{
    public class ResilientHttpStartupTest : BaseStartupTest
    {
        public ResilientHttpStartupTest(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            //services.AddMetrics(configuration.GetSection("AppMetrics"));
            services.AddFlash(flash =>
            {
                flash.AddResilientHttpClient((aorign, option) =>
                {
                    option.DurationSecondsOfBreak = 30;
                    option.ExceptionsAllowedBeforeBreaking = 5;
                    option.RetryCount = 5;
                    option.TimeoutMillseconds = 10000;
                });
            });
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);
        }
    }
}
