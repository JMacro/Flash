using Flash.Extensions.ChangeHistory;
using Flash.Extensions.Office;
using Flash.Extensions.ORM;
using Flash.Test.EntityChange.Events;
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
                flash.AddEntityChange(setup =>
                {
                    setup.InitConfig(config =>
                    {
                        config.MaxDifferences = int.MaxValue;
                    });

                    var sp = setup.Services.BuildServiceProvider();
                });

                flash.AddORM(orm =>
                {
                    orm.UseEntityFramework(option =>
                    {
                        var connection = Environment.GetEnvironmentVariable("MySQL_Connection", EnvironmentVariableTarget.Machine);
                        option.RegisterDbContexts<TestDbContext, MigrationAssembly>(connection, Configuration);
                        option.RegisterGlobalEvents(events =>
                        {
                            events.StateChanged = (EntityChangeTracker entityChangeTracker) =>
                            {
                                var sp = option.Services.BuildServiceProvider();
                                var entityChange = sp.GetService<IEntityChange>();
                                Console.WriteLine("State:{0} OriginalEntity:{1} CurrentEntity:{2}", entityChangeTracker.State, Newtonsoft.Json.JsonConvert.SerializeObject(entityChangeTracker.OriginalEntity), Newtonsoft.Json.JsonConvert.SerializeObject(entityChangeTracker.CurrentEntity));
                                var compareResult = entityChange.Compare(Guid.NewGuid(), entityChangeTracker.OriginalEntity, entityChangeTracker.CurrentEntity);
                                if (compareResult != null)
                                {
                                    Console.WriteLine("本次变更属性：{0}", string.Join(",", compareResult.HistoryPropertys.Select(p => p.PropertyName)));
                                }

                            };
                        });
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
