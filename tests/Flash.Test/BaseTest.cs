using Autofac;
using Autofac.Extensions.DependencyInjection;
using Flash.Extensions;
using Flash.Extensions.Email;
using Flash.Extensions.Office;
using Flash.Test.ORM.Base;
using Flash.Test.ORM.Events;
using FluentAssertions.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Flash.Test
{
    public class BaseTest
    {
        protected IContainer container;
        private IServiceCollection services;

        public ServiceProvider ServiceProvider { get; private set; }


        public BaseTest()
        {
        }


        #region Setup/Teardown

        /// <summary>
        /// Code that is run before each test
        /// </summary>
        [SetUp]
        public void Initialize()
        {
            var Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .SetEnvironmentVariable("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                .AddJsonFileEx("Config/appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            //创建服务容器对象
            services = new ServiceCollection();

            services.AddLogging();

            //services.AddMetrics(configuration.GetSection("AppMetrics"));
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
                        .WithKeyPrefix("JMacro:Flash:Tests");
                    });
                });


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
                    orm.UseEntityFramework(option =>
                    {
                        var connection = Environment.GetEnvironmentVariable("MySQL_Connection", EnvironmentVariableTarget.Machine);
                        option.RegisterDbContexts<TestDbContext, MigrationAssembly>(connection, Configuration);
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

                flash.AddOffice(setting =>
                {
                    setting.WithDefaultExcelSetting(new SheetSetting
                    {
                        IsAutoNumber = true
                    });
                }, setup =>
                {
                    setup.UseNpoi();
                });

                flash.AddEntityChange(setup =>
                {
                    setup.InitConfig(config =>
                    {
                        config.MaxDifferences = int.MaxValue;
                    });
                    setup.UseRabbitMQStorage<ChangeHistoryMessageHandler>();
                });
            });

            //构建ServiceProvider对象
            ServiceProvider = services.BuildServiceProvider();

            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            container = containerBuilder.Build();
        }

        /// <summary>
        /// Code that is run after each test
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
        }
        #endregion
    }

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

        }
    }
}
