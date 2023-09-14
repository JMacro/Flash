using Flash.Extensions.ChangeHistory;
using Flash.Extensions.EventBus;
using Flash.Extensions.EventBus.RabbitMQ;
using Flash.Extensions.ORM;
using Flash.Test.EntityChange.Events;
using Flash.Test.Events.Messages;
using Flash.Test.ORM;
using Flash.Test.ORM.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Flash.Test.StartupTests
{
    public class EFStartupTest : BaseStartupTest
    {
        public EFStartupTest(IConfiguration configuration) : base(configuration)
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
                flash.AddEventBus(bus =>
                {
                    bus.UseRabbitMQ(rabbitmq =>
                    {
                        var hostName = Environment.GetEnvironmentVariable("RabbitMQ:HostName", EnvironmentVariableTarget.Machine);
                        var port = Environment.GetEnvironmentVariable("RabbitMQ:Port", EnvironmentVariableTarget.Machine);
                        var userName = Environment.GetEnvironmentVariable("RabbitMQ:UserName", EnvironmentVariableTarget.Machine);
                        var password = Environment.GetEnvironmentVariable("RabbitMQ:Password", EnvironmentVariableTarget.Machine);
                        var virtualHost = Environment.GetEnvironmentVariable("RabbitMQ:VirtualHost", EnvironmentVariableTarget.Machine);

                        rabbitmq.WithEndPoint(hostName ?? "localhost", int.Parse(port ?? "5672"))
                        .WithPrefixName("自定义前缀")
                        .WithAuth(userName ?? "guest", password ?? "guest")
                        .WithExchange(virtualHost ?? "/", Exchange: $"{GetType().FullName}")
                        .WithSender(int.Parse(Configuration["RabbitMQ:SenderMaxConnections"] ?? "10"), int.Parse(Configuration["RabbitMQ:SenderAcquireRetryAttempts"] ?? "3"))
                        .WithReceiver(
                            ReceiverMaxConnections: int.Parse(Configuration["RabbitMQ:ReceiverMaxConnections"] ?? "5"),
                            ReveiverMaxDegreeOfParallelism: int.Parse(Configuration["RabbitMQ:ReveiverMaxDegreeOfParallelism"] ?? "5"),
                            ReceiverAcquireRetryAttempts: int.Parse(Configuration["RabbitMQ:ReceiverAcquireRetryAttempts"] ?? "3"));
                    });
                });

                flash.AddEntityChange(setup =>
                {
                    setup.InitConfig(config =>
                    {
                        config.MaxDifferences = int.MaxValue;
                    });

                    var sp = setup.Services.BuildServiceProvider();
                    setup.UseRabbitMQStorage<ChangeHistoryMessageHandler>(sp.GetService<IEventBus>());
                });

                flash.AddORM(orm =>
                {
                    orm.UseEntityFramework(option =>
                    {
                        var connection = Environment.GetEnvironmentVariable("MySQL_Connection", EnvironmentVariableTarget.Machine);
                        option.RegisterDbContexts<TestDb1Context, MigrationAssembly>(connection, Configuration);
                        option.RegisterDbContexts<TestDb2Context, MigrationAssembly>(connection, Configuration);
                        option.RegisterGlobalEvents(events =>
                        {
                            events.StateChanged = (EntityChangeTracker entityChangeTracker) =>
                            {
                                var sp = option.Services.BuildServiceProvider();
                                var entityChange = sp.GetService<IEntityChange>();
                                Console.WriteLine("State:{0} OriginalEntity:{1} CurrentEntity:{2}", entityChangeTracker.State, Newtonsoft.Json.JsonConvert.SerializeObject(entityChangeTracker.OriginalEntity), Newtonsoft.Json.JsonConvert.SerializeObject(entityChangeTracker.CurrentEntity));
                                var result = entityChange.Record(Guid.NewGuid(), entityChangeTracker.OriginalEntity, entityChangeTracker.CurrentEntity).ConfigureAwait(false).GetAwaiter().GetResult();
                                if (result)
                                {
                                    Console.WriteLine("本次变更属性：{0}", string.Join(",", entityChange.ChangeHistoryInfo.HistoryPropertys.Select(p => p.PropertyName)));
                                }
                            };
                        });
                    });
                });
            });
        }

        public override void Configure(IApplicationBuilder app)
        {
        }
    }
}
