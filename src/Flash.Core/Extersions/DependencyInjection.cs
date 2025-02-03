using Autofac;
using Autofac.Extensions.DependencyInjection;
using Flash.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        /// <summary>
        /// 使用服务注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setup"></param>
        /// <param name="containerBuilder"></param>
        public static IServiceCollection AddFlash(this IServiceCollection services, Action<IFlashHostBuilder> setup, ContainerBuilder containerBuilder = null)
        {
            var builder = new FlashHostBuilder(services, containerBuilder);
            setup(builder);
            MicrosoftContainer.Instance = services.BuildServiceProvider();
            builder.Container.Populate(services);
            AutofacContainer.Instance = builder.Container.Build();
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IFlashApplicationBuilder UseFlash(this IApplicationBuilder app, Action<IFlashApplicationBuilder> setup)
        {
            var builder = new FlashApplicationBuilder(app);
            setup(builder);
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webHostBuilder"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IWebHostBuilder ConfigureFlash(this IWebHostBuilder webHostBuilder, Action<IFlashWebHostBuilder> setup)
        {
            var builder = new FlashWebHostBuilder(webHostBuilder);
            setup(builder);
            return webHostBuilder;
        }
    }
}
