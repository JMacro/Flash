using Flash.Extensions.EventBus;
using Flash.Extensions.Office;
using Flash.Test.EntityChange.Events;
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
    public class EntityChangeStartupTest : BaseStartupTest
    {
        public EntityChangeStartupTest(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
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
            });
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);
        }
    }
}
