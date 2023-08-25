using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Test.StartupTests
{
    public class UniqueIdGeneratorStartupTest : BaseStartupTest
    {
        public UniqueIdGeneratorStartupTest(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            //services.AddMetrics(configuration.GetSection("AppMetrics"));
            services.AddFlash(flash =>
            {
                flash.AddUniqueIdGenerator(setup =>
                {
                    setup.CenterId = 0;
                    setup.UseStaticWorkIdCreateStrategy(0);
                });
            });
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);
        }
    }
}
