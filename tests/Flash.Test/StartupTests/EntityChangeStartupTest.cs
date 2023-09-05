using Flash.Extensions.ChangeHistory;
using Flash.Extensions.EventBus;
using Flash.Extensions.Office;
using Flash.Test.EntityChange.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Test.StartupTests
{
    public class EntityChangeStartupTest : BaseStartupTest
    {
        public EntityChangeStartupTest(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });
            services.AddFlash(flash =>
            {
                flash.AddEntityChange<DefaultStorage>(setup =>
                {
                    setup.InitConfig(config =>
                    {
                        config.MaxDifferences = int.MaxValue;
                    });
                });
            });
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);
        }
    }
}
