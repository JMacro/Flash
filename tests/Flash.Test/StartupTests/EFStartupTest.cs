using Flash.Extensions.Office;
using Flash.Test.ORM.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Test.StartupTests
{
    public class EFStartupTest : BaseStartupTest
    {
        public EFStartupTest(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddFlash(flash =>
            {
                flash.AddORM(orm =>
                {
                    orm.UseEntityFramework(option =>
                    {
                        var connection = Environment.GetEnvironmentVariable("MySQL_Connection", EnvironmentVariableTarget.Machine);
                        option.RegisterDbContexts<TestDbContext, MigrationAssembly>(connection, Configuration);
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
