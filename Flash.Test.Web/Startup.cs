using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Flash.Test.Web.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAutofac();
            services.AddFlash(flash =>
            {
                flash.AddSecurity3DES(setup =>
                {
                    setup.Encoding = Encoding.UTF8;
                    setup.SecretKey = "sdfdsfsdf";
                });
                flash.AddRabbitMQ(rabbitmq =>
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
                app.UseHsts();
            }

            var eventBus = app.ApplicationServices.GetRequiredService<Extersions.RabbitMQ.IBus>();
            app.UseFlash(flash =>
            {
                flash.UseRabbitMQ(rabbitmq =>
                {
                    rabbitmq.Register<Controllers.OrderInfo, OrderInfoHandler>(queueName: typeof(OrderInfoHandler).FullName);
                });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
