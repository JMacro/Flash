using Autofac;
using Autofac.Extensions.DependencyInjection;
using Flash.Extensions.EventBus;
using Flash.Extensions.EventBus.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;

namespace Flash.Test.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAutofac();
            services.AddMetrics(Configuration.GetSection("AppMetrics"));
            services.AddFlash(flash =>
            {
                flash.AddSecurity3DES(setup =>
                {
                    setup.Encoding = Encoding.UTF8;
                    setup.SecretKey = "sdfdsfsdf";
                });

                flash.AddEventBus(bus =>
                {
                    bus.AddRabbitMQ(rabbitmq =>
                    {
                        rabbitmq.WithEndPoint(Configuration["RabbitMQ:HostName"] ?? "localhost", int.Parse(Configuration["RabbitMQ:Port"] ?? "5672"))
                        .WithAuth(Configuration["RabbitMQ:UserName"] ?? "guest", Configuration["RabbitMQ:Password"] ?? "guest")
                        .WithExchange(Configuration["RabbitMQ:VirtualHost"] ?? "/")
                        .WithSender(int.Parse(Configuration["RabbitMQ:SenderMaxConnections"] ?? "10"), int.Parse(Configuration["RabbitMQ:SenderAcquireRetryAttempts"] ?? "3"))
                        .WithReceiver(
                            ReceiverMaxConnections: int.Parse(Configuration["RabbitMQ:ReceiverMaxConnections"] ?? "5"),
                            ReveiverMaxDegreeOfParallelism: int.Parse(Configuration["RabbitMQ:ReveiverMaxDegreeOfParallelism"] ?? "5"),
                            ReceiverAcquireRetryAttempts: int.Parse(Configuration["RabbitMQ:ReceiverAcquireRetryAttempts"] ?? "3"));
                    });
                });
            });
            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            var logger = app.ApplicationServices.GetRequiredService<ILogger<IEventBus>>();
            app.UseFlash(flash =>
            {
                flash.UseEventBus(sp =>
                {
                    sp.UseSubscriber(eventbus =>
                    {
                        eventbus.Register<TestEvent, TestEventHandler>("TestEventHandler", "routerkey.log.error");
                        eventbus.Register<TestEvent2, TestEvent2Handler>("TestEventHandler2", "routerkey.log.info");

                        //订阅消息
                        eventbus.Subscriber((Messages) =>
                        {
                            foreach (var message in Messages)
                            {
                                logger.LogDebug($"ACK: queue {message.QueueName} route={message.RouteKey} messageId:{message.MessageId}");
                            }

                        }, async (obj) =>
                        {
                            foreach (var message in obj.Messages)
                            {
                                logger.LogError($"NAck: queue {message.QueueName} route={message.RouteKey} messageId:{message.MessageId}");
                            }

                            //消息消费失败执行以下代码
                            if (obj.Exception != null)
                            {
                                logger.LogError(obj.Exception, obj.Exception.Message);
                            }

                            var events = obj.Messages.Select(message => message.WaitAndRetry(a => 5, 3)).ToList();

                            var ret = !(await eventBus.PublishAsync(events));

                            return ret;
                        });
                    });
                });
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
