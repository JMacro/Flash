using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Extensions;
using System.IO;

namespace Flash.Test.StartupTests
{
    public class RulesEngineStartupTest : BaseStartupTest
    {
        public RulesEngineStartupTest(IConfiguration configuration) : base(configuration)
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
                flash.AddRulesEngine(config =>
                {
                    config.ReSettings = new RulesEngine.Models.ReSettings
                    {
                    };
                    config.UseLoaclFileStorage(this.Configuration);
                });
            });
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);
        }
    }
}
