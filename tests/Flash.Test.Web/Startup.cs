using Autofac;
using Autofac.Extensions.DependencyInjection;
using Flash.Core;
using Flash.Extensions.EventBus;
using Flash.Extensions.EventBus.RabbitMQ;
using Flash.Extensions.HealthChecks;
using Flash.Test.Web;
using Flash.Test.Web.EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Text;


[assembly: HostingStartup(typeof(SkyApmHostingStartup))]

namespace Flash.Test.Web
{
    internal class SkyApmHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            var dd = 12;
        }
    }
}

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
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                options.SerializerSettings.Converters.Add(new NumberConverter(NumberConverterShip.Int64));
                options.SerializerSettings.Converters.Add(new DesensitizationConverter());
            });
            services.AddAutofac();
            services.AddMetrics(Configuration.GetSection("AppMetrics"));
            services.AddHealthChecks(checks =>
            {
                checks.WithDefaultCacheDuration(TimeSpan.FromSeconds(10));

                var host = Environment.GetEnvironmentVariable("Redis_Host", EnvironmentVariableTarget.Machine);
                var password = Environment.GetEnvironmentVariable("Redis_Password", EnvironmentVariableTarget.Machine);
                //checks.AddRedisCheck("redis1", $"{host},password={password},allowAdmin=true,ssl=false,abortConnect=false,connectTimeout=5000");

                var connection = Environment.GetEnvironmentVariable("MySQL_Connection", EnvironmentVariableTarget.Machine);
                checks.AddMySqlCheck("MySql1", connection);

                checks.AddRabbitMQCheck("RabbitMQ", setup =>
                {

                });
            });

            services.AddFlash(flash =>
            {
                flash.AddSecurity3DES(setup =>
                {
                    setup.Encoding = Encoding.UTF8;
                    setup.SecretKey = "sdfdsfsdf";
                });

                flash.AddUniqueIdGenerator(setup =>
                {
                    setup.CenterId = 0;
                    setup.UseStaticWorkIdCreateStrategy(0);
                });

                flash.AddCache(cache =>
                {
                    var host = Environment.GetEnvironmentVariable("Redis_Host", EnvironmentVariableTarget.Machine);
                    var password = Environment.GetEnvironmentVariable("Redis_Password", EnvironmentVariableTarget.Machine);

                    cache.UseRedis(option =>
                    {
                        option.WithNumberOfConnections(5)
                        .WithWriteServerList(host)
                        .WithReadServerList(host)
                        .WithDb(0)
                        .WithDistributedLock(true)
                        .WithPassword(password)
                        .WithKeyPrefix("JMacro");
                    });
                });


                flash.AddEventBus(bus =>
                {
                    bus.UseRabbitMQ(rabbitmq =>
                    {
                        //rabbitmq
                        //.WithEndPoint(Configuration["RabbitMQ:HostName"] ?? "localhost", int.Parse(Configuration["RabbitMQ:Port"] ?? "5672"))
                        //.WithAuth(Configuration["RabbitMQ:UserName"] ?? "guest", Configuration["RabbitMQ:Password"] ?? "guest")
                        //.WithExchange(Configuration["RabbitMQ:VirtualHost"] ?? "/")
                        //.WithSender(int.Parse(Configuration["RabbitMQ:SenderMaxConnections"] ?? "10"), int.Parse(Configuration["RabbitMQ:SenderAcquireRetryAttempts"] ?? "3"))
                        //.WithReceiver(
                        //    ReceiverMaxConnections: int.Parse(Configuration["RabbitMQ:ReceiverMaxConnections"] ?? "5"),
                        //    ReveiverMaxDegreeOfParallelism: int.Parse(Configuration["RabbitMQ:ReveiverMaxDegreeOfParallelism"] ?? "5"),
                        //    ReceiverAcquireRetryAttempts: int.Parse(Configuration["RabbitMQ:ReceiverAcquireRetryAttempts"] ?? "3"));

                        var hostName = Environment.GetEnvironmentVariable("RabbitMQ:HostName", EnvironmentVariableTarget.Machine);
                        var port = Environment.GetEnvironmentVariable("RabbitMQ:Port", EnvironmentVariableTarget.Machine);
                        var userName = Environment.GetEnvironmentVariable("RabbitMQ:UserName", EnvironmentVariableTarget.Machine);
                        var password = Environment.GetEnvironmentVariable("RabbitMQ:Password", EnvironmentVariableTarget.Machine);
                        var virtualHost = Environment.GetEnvironmentVariable("RabbitMQ:VirtualHost", EnvironmentVariableTarget.Machine);

                        rabbitmq.WithEndPoint(hostName ?? "localhost", int.Parse(port ?? "5672"))
                        .WithPrefixName("")
                        .WithAuth(userName ?? "guest", password ?? "guest")
                        .WithExchange(virtualHost ?? "/", Exchange: $"{this.GetType().FullName}")
                        .WithSender(int.Parse(Configuration["RabbitMQ:SenderMaxConnections"] ?? "10"), int.Parse(Configuration["RabbitMQ:SenderAcquireRetryAttempts"] ?? "3"))
                        .WithReceiver(
                            ReceiverMaxConnections: int.Parse(Configuration["RabbitMQ:ReceiverMaxConnections"] ?? "5"),
                            ReveiverMaxDegreeOfParallelism: int.Parse(Configuration["RabbitMQ:ReveiverMaxDegreeOfParallelism"] ?? "5"),
                            ReceiverAcquireRetryAttempts: int.Parse(Configuration["RabbitMQ:ReceiverAcquireRetryAttempts"] ?? "3"));
                    });
                });


                //flash.AddLoggerTracing(tracer =>
                //{
                //    tracer.UseJaeger(Configuration.GetSection("Tracing"));
                //    tracer.UseSkywalking("Flash.Test.Web");
                //});

                flash.AddORM(orm =>
                {
                    orm.UseEFCore<TestDbContext>(option =>
                    {
                        var connection = Environment.GetEnvironmentVariable("MySQL_Connection", EnvironmentVariableTarget.Machine);
                        option.UseMySql(connection, ServerVersion.AutoDetect(connection));
                    });
                });

                flash.AddResilientHttpClient((aorign, option) =>
                {
                    option.DurationSecondsOfBreak = 30;
                    option.ExceptionsAllowedBeforeBreaking = 5;
                    option.RetryCount = 5;
                    option.TimeoutMillseconds = 10000;
                });

                flash.AddJob(job =>
                {
                    //job.UseQuartz();
                    job.UseHangfire();
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

            //var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            var logger = app.ApplicationServices.GetRequiredService<ILogger<IEventBus>>();
            app.UseFlash(flash =>
            {
                //flash.UseHangfire();
                flash.UseEventBus(sp =>
                {
                    sp.UseSubscriber(eventbus =>
                    {
                        eventbus.RegisterWaitAndRetry<TestEvent, TestEventHandler>("", "");
                        //eventbus.Register<TestEvent, TestEventHandler>(typeof(TestEventHandler).FullName, "routerkey.log.error");
                        eventbus.Register<TestEvent2, TestEvent2Handler>(typeof(TestEvent2Handler).FullName, "routerkey.log.*");
                        eventbus.RegisterDelay<TestDelayMessage, TestDelayHandler>();

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

                            var ret = !(await eventbus.PublishAsync(events));

                            return ret;
                        });
                    });
                });
            });
            //app.UseEventBusDashboard();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseMvc();
            app.UseRouting();

            app.UseEndpoints(option =>
            {
                option.MapControllers();
            });
        }
    }
}
