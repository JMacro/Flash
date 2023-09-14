using Flash.Core;
using Flash.Extensions.DynamicRoute.Nacos;
using Microsoft.Extensions.Configuration;
using Nacos.AspNetCore.V2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        private static IServiceCollection AddNacosDynamicRoute(this IServiceCollection services, NacosConfig config)
        {
            services.AddNacosAspNet(a =>
            {
                a.Ephemeral = config.Ephemeral;
                a.Ip = config.Ip;
                a.Metadata = config.Metadata;
                a.Port = config.Port;
                a.Secure = config.Secure;
                a.ClusterName = config.ClusterName;
                a.GroupName = config.GroupName;
                a.Weight = config.Weight;
                a.ServiceName = config.ServiceName;
                a.Namespace = config.Namespace;
                a.Password = config.Password;
                a.UserName = config.UserName;
                a.AccessKey = config.AccessKey;
                a.ContextPath = config.ContextPath;
                a.EndPoint = config.EndPoint;
                a.ListenInterval = config.ListenInterval;
                a.SecretKey = config.SecretKey;
                a.ServerAddresses = config.ServerAddresses;
                a.ConfigFilterAssemblies = config.ConfigFilterAssemblies;
                a.ConfigUseRpc = config.ConfigUseRpc;
                a.DefaultTimeOut = config.DefaultTimeOut;
                a.NamingUseRpc = config.NamingUseRpc;
                a.RamRoleName = config.RamRoleName;
                a.ConfigFilterExtInfo = config.ConfigFilterExtInfo;
                a.NamingCacheRegistryDir = config.NamingCacheRegistryDir;
                a.NamingLoadCacheAtStart = config.NamingLoadCacheAtStart;
                a.NamingPushEmptyProtection = config.NamingPushEmptyProtection;
            });
            services.AddHostedService<RegSvcBgTask>();
            return services;
        }

        private static IServiceCollection AddNacosDynamicRoute(this IServiceCollection services, Action<NacosConfig> configure)
        {
            NacosConfig config = new NacosConfig();

            if (configure != null)
            {
                configure(config);
            }
            return services.AddNacosDynamicRoute(config);
        }

        public static IFlashHostBuilder AddNacosDynamicRoute(this IFlashHostBuilder hostBuilder, Action<NacosConfig> configure)
        {
            hostBuilder.Services.AddNacosDynamicRoute(configure);
            return hostBuilder;
        }
        public static IFlashHostBuilder AddNacosDynamicRoute(this IFlashHostBuilder hostBuilder, IConfiguration configuration)
        {
            hostBuilder.Services.AddNacosDynamicRoute(configuration.Get<NacosConfig>());
            return hostBuilder;
        }
    }
}
