using Flash.Core;
using Flash.Extensions.Resilience.Http;
using Flash.Extensions.Tracting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 弹性Http客户端
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddResilientHttpClient(this IFlashHostBuilder hostBuilder, Action<string, ResilientHttpClientConfigOption> func = null)
        {
            hostBuilder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            hostBuilder.Services.TryAddSingleton<ITracerFactory, TracerFactory>();
            hostBuilder.Services.TryAddSingleton<IHttpClientFactory>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ResilientHttpClient>>();
                logger.LogInformation("Using ResilientHttpClient");
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var serviceLocator = sp.GetService<Flash.DynamicRoute.IServiceLocator>();
                var tracerFactory = sp.GetService<ITracerFactory>();

                return new ResilientHttpClientFactory(logger,
                    httpContextAccessor,
                    serviceLocator,
                    tracerFactory,
                    func);
            });
            hostBuilder.Services.TryAddSingleton<IHttpClient>(sp => sp.GetService<IHttpClientFactory>().CreateResilientHttpClient());
            return hostBuilder;

        }

        /// <summary>
        /// 弹性Http客户端
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="func"></param>
        /// <param name="httpMessageHandler"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddResilientHttpClient(this IFlashHostBuilder hostBuilder, Action<string, ResilientHttpClientConfigOption> func = null, HttpMessageHandler httpMessageHandler = null)
        {
            hostBuilder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            hostBuilder.Services.TryAddSingleton<ITracerFactory, TracerFactory>();
            hostBuilder.Services.TryAddSingleton<IHttpClientFactory>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ResilientHttpClient>>();
                logger.LogInformation("Using ResilientHttpClient");
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var serviceLocator = sp.GetService<Flash.DynamicRoute.IServiceLocator>();
                var tracerFactory = sp.GetService<ITracerFactory>();

                return new ResilientHttpClientFactory(logger,
                    httpContextAccessor,
                    serviceLocator,
                    tracerFactory,
                    func);
            });
            hostBuilder.Services.TryAddSingleton<IHttpClient>(sp => sp.GetService<IHttpClientFactory>().CreateResilientHttpClient(httpMessageHandler));
            return hostBuilder;

        }

        /// <summary>
        /// 标准Http客户端
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddStandardHttpClient(this IFlashHostBuilder hostBuilder)
        {
            hostBuilder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            hostBuilder.Services.TryAddSingleton<ITracerFactory, TracerFactory>();
            hostBuilder.Services.TryAddSingleton<IHttpClientFactory>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<StandardHttpClient>>();
                logger.LogInformation("Using StandardHttpClient");
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var serviceLocator = sp.GetService<Flash.DynamicRoute.IServiceLocator>();
                var tracerFactory = sp.GetService<ITracerFactory>();

                return new StandardHttpClientFactory(logger, httpContextAccessor, serviceLocator, tracerFactory);
            });
            hostBuilder.Services.TryAddSingleton<IHttpClient>(sp => sp.GetService<IHttpClientFactory>().CreateResilientHttpClient());
            return hostBuilder;

        }

        /// <summary>
        /// 标准Http客户端
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="httpMessageHandler"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddStandardHttpClient(this IFlashHostBuilder hostBuilder, HttpMessageHandler httpMessageHandler)
        {
            hostBuilder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            hostBuilder.Services.TryAddSingleton<ITracerFactory, TracerFactory>();
            hostBuilder.Services.TryAddSingleton<IHttpClientFactory>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<StandardHttpClient>>();
                logger.LogInformation("Using StandardHttpClient");
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var serviceLocator = sp.GetService<Flash.DynamicRoute.IServiceLocator>();
                var tracerFactory = sp.GetService<ITracerFactory>();

                return new StandardHttpClientFactory(logger, httpContextAccessor, serviceLocator, tracerFactory);
            });
            hostBuilder.Services.TryAddSingleton<IHttpClient>(sp => sp.GetService<IHttpClientFactory>().CreateResilientHttpClient(httpMessageHandler));
            return hostBuilder;

        }
    }
}
