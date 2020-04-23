using Flash.Core;
using System;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        /// <summary>
        /// 使用服务注册
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddFlash(this IServiceCollection services, Action<IFlashHostBuilder> setup)
        {
            var builder = new FlashHostBuilder(services);
            setup(builder);
            return services;
        }


        public static IFlashApplicationBuilder UseFlash(this IApplicationBuilder app, Action<IFlashApplicationBuilder> setup)
        {
            var builder = new FlashApplicationBuilder(app);
            setup(builder);
            return builder;
        }

    }
}
