using Autofac;
using Autofac.Extensions.DependencyInjection;
using Flash.Extensions;
using Flash.Extensions.Email;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Flash.Test
{
    public class BaseTest
    {
        protected IContainer container;
        private IServiceCollection services;

        public ServiceProvider ServiceProvider { get; private set; }


        public BaseTest()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .SetEnvironmentVariable("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                .AddJsonFileEx("Config/appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            var d = configuration.GetSection("ConnectionStrings:RedisConnectionString").Value;

            //创建服务容器对象
            services = new ServiceCollection();

            //services.AddMetrics(configuration.GetSection("AppMetrics"));
            services.AddFlash(setup =>
            {
                //setup.AddRedisDistributedLock(option =>
                //{
                //    option.WithDb(10);
                //    option.WithKeyPrefix("SystemClassName:TypeClassName");
                //    option.WithPassword("tY7cRu9HG_jyDw2r");
                //    option.WithReadServerList("192.168.109.237:63100");
                //    option.WithWriteServerList("192.168.109.237:63100");
                //    option.WithSsl(false);
                //    option.WithDistributedLock(true);
                //});

                setup.AddCache(cache =>
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
                        .WithPassword(password);
                    });
                });

                setup.AddOffice(setting =>
                {
                    setting.WithDefaultExcelSetting(new Extensions.Office.SheetSetting
                    {

                    });
                }, action =>
                {
                    action.UseNpoi();
                });

                //setup.AddOpenTracing(option =>
                //{
                //    //option.AddJaeger(ation =>
                //    //{

                //    //});
                //});

                setup.AddMailKit(configuration.GetSection("Email").Get<EmailConfig>());

            });

            //构建ServiceProvider对象
            ServiceProvider = services.BuildServiceProvider();

            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            container = containerBuilder.Build();
        }
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
