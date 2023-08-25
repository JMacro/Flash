using Flash.Extensions.Office;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Test.StartupTests
{
    public class OfficeStartupTest : BaseStartupTest
    {
        public OfficeStartupTest(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            //services.AddMetrics(configuration.GetSection("AppMetrics"));
            services.AddFlash(flash =>
            {
                flash.AddOffice(setting =>
                {
                    setting.WithDefaultExcelSetting(new SheetSetting
                    {
                        IsAutoNumber = true
                    });
                }, setup =>
                {
                    setup.UseNpoi();
                });
            });
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);
        }
    }
}
