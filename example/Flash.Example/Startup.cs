using Autofac;
using Flash.AspNetCore;
using Flash.AspNetCore.WorkFlow.Application.Abastracts.Services;
using Flash.AspNetCore.WorkFlow.Application.Dtos.FlowConfigs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Flash.Example
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigServices(IServiceCollection services)
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            services.AddControllers();
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

                flash.AddWorkFlow(workFlow =>
                {
                    var connection = Environment.GetEnvironmentVariable("MySQL_Connection_Workflow", EnvironmentVariableTarget.Machine);
                    workFlow.RegisterDbContext<MigrationAssembly>(connection);
                });
            }, containerBuilder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void ConfigApplication(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthorization();

            var dd = MicrosoftContainer.Instance.GetService<IFlowConfigService>();
            dd.Init(() =>
            {
                return new List<FlowConfigRequestDto>
                {
                    new FlowConfigRequestDto { Id = 1,Name="sdf" }
                };
            });
        }
    }
}
