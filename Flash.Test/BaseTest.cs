using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Test
{
    public class BaseTest
    {
        protected IContainer container;

        public BaseTest()
        {
            //创建服务容器对象
            var services = new ServiceCollection();
            services.AddFlash(setup =>
            {
                setup.AddRedis(option =>
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

            //构建ServiceProvider对象
            var serviceProvider = services.BuildServiceProvider();

            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            container = containerBuilder.Build();
        }
    }
}
