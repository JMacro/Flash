using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Flash.Test
{
    public class BaseTest
    {
        protected IContainer container;

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
            var services = new ServiceCollection();
            services.AddFlash(setup =>
            {
                setup.AddCache(cache =>
                {
                    cache.AddRedis(option =>
                    {
                        option.WithDb(10);
                        option.WithKeyPrefix("SystemClassName:TypeClassName");
                        option.WithPassword("tY7cRu9HG_jyDw2r");
                        option.WithReadServerList("192.168.109.237:63100");
                        option.WithWriteServerList("192.168.109.237:63100");
                        option.WithSsl(false);
                        option.WithHealthyCheck(false);
                    })
                    .AddDistributedLock();
                });

                setup.AddOpenTracing(option =>
                {
                    option.AddJaeger(ation =>
                    {

                    });
                });

            });

            //构建ServiceProvider对象
            var serviceProvider = services.BuildServiceProvider();

            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            container = containerBuilder.Build();
        }
    }
}
