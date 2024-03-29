﻿using Flash.AspNetCore.Enums;
using Flash.Extensions.OpenTracting;
using Flash.Extensions.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Consul;
using Flash.Extensions.UidGenerator.ConsulWorkId;
using Flash.Extensions;
using Flash.Extensions.Cache;
using Flash.Extensions.UidGenerator.RedisWorkId;
using Flash.Extensions.UidGenerator.WorkIdCreateStrategy;
using Microsoft.Extensions.Hosting;
using static System.Net.WebRequestMethods;

namespace Flash.AspNetCore
{
    /// <summary>
    /// 应用启动配置入口
    /// </summary>
    /// <remarks>
    /// See <see href="https://github.com/JMacro/Flash/blob/master/README/README.AspNetCore.md">AspNetCore configuration</see> for more information.
    /// </remarks>
    public abstract class BaseStartup
    {
        #region 属性
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }
        #endregion

        public BaseStartup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.Configuration = configuration;
            this.Env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            this.ConfigServices(services);

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<BaseStartup>>();

            services.AddFlash(flash =>
            {
                #region Consul
                var enableConsul = this.Configuration.GetSection("FlashConfiguration:Consul:Enable").Get<bool?>();
                if (enableConsul.HasValue && enableConsul.Value)
                {
                    flash.AddConsulDynamicRoute(this.Configuration.GetSection("FlashConfiguration:Consul:Config"), setup =>
                    {
                        setup.AddTags(this.Configuration.GetSection("FlashConfiguration:Consul:Config:SERVICE_TAGS").Get<string>());
                    });
                }
                #endregion

                #region Nacos
                var enableNacos = this.Configuration.GetSection("FlashConfiguration:Nacos:Enable").Get<bool?>();
                if (enableNacos.HasValue && enableNacos.Value)
                {
                    flash.AddNacosDynamicRoute(this.Configuration.GetSection("FlashConfiguration:Nacos:Config"));
                }
                #endregion

                #region 缓存
                var enableCache = this.Configuration.GetSection("FlashConfiguration:Cache:Enable").Get<bool?>();
                var cacheType = this.Configuration.GetSection("FlashConfiguration:Cache:CacheType").Get<EFlashCache4CacheType>();
                if (enableCache.HasValue && enableCache.Value && cacheType != EFlashCache4CacheType.None)
                {
                    flash.AddCache(cache =>
                    {
                        switch (cacheType)
                        {
                            case EFlashCache4CacheType.Redis:
                                var host = this.Configuration.GetSection("FlashConfiguration:Cache:RedisConfig:Host").Get<string>();
                                var password = this.Configuration.GetSection("FlashConfiguration:Cache:RedisConfig:Password").Get<string>();
                                var dbNum = this.Configuration.GetSection("FlashConfiguration:Cache:RedisConfig:Db").Get<int>();
                                var distributedLock = this.Configuration.GetSection("FlashConfiguration:Cache:RedisConfig:DistributedLock").Get<bool>();
                                var keyPrefix = this.Configuration.GetSection("FlashConfiguration:Cache:RedisConfig:KeyPrefix").Get<string>();

                                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(password)) throw new ArgumentException($"appsettings.json配置文件未设置【FlashConfiguration:Cache:RedisConfig:Host 或 FlashConfiguration:Cache:RedisConfig:Password】");

                                cache.UseRedis(option =>
                                {
                                    option.WithNumberOfConnections(5)
                                    .WithWriteServerList(host)
                                    .WithReadServerList(host)
                                    .WithDb(dbNum)
                                    .WithDistributedLock(distributedLock, distributedLock)
                                    .WithPassword(password)
                                    .WithKeyPrefix(keyPrefix);
                                });
                                break;
                        }
                    });
                }
                #endregion

                #region 唯一Id生成器
                var enableUniqueIdGenerator = this.Configuration.GetSection("FlashConfiguration:UniqueIdGenerator:Enable").Get<bool?>();
                var generatorType = this.Configuration.GetSection("FlashConfiguration:UniqueIdGenerator:GeneratorType").Get<EFlashUniqueIdGenerator4GeneratorType>();

                if (enableUniqueIdGenerator.HasValue && enableUniqueIdGenerator.Value && generatorType != EFlashUniqueIdGenerator4GeneratorType.None)
                {
                    var centerId = this.Configuration.GetSection("FlashConfiguration:UniqueIdGenerator:CenterId").Get<int?>();
                    var workId = this.Configuration.GetSection("FlashConfiguration:UniqueIdGenerator:WorkId").Get<int?>();

                    flash.AddUniqueIdGenerator(setup =>
                    {
                        var appId = this.Configuration.GetSection("FlashConfiguration:UniqueIdGenerator:AppId").Get<string>();
                        switch (generatorType)
                        {
                            case EFlashUniqueIdGenerator4GeneratorType.StaticWorkId:
                                if (!centerId.HasValue || !workId.HasValue) throw new ArgumentException("appsettings.json配置文件未设置【FlashConfiguration:UniqueIdGenerator:CenterId 与 FlashConfiguration:UniqueIdGenerator:WorkId】");
                                setup.CenterId = centerId.Value;
                                setup.UseStaticWorkIdCreateStrategy(workId.Value);
                                break;
                            case EFlashUniqueIdGenerator4GeneratorType.ConsulWorkId:
                                var consulClient = flash.Services.BuildServiceProvider().GetService<IConsulClient>();
                                Check.Argument.IsNotNull(consulClient, nameof(IConsulClient), "未注册Consul组件，请添加Consul配置信息");

                                setup.CenterId = centerId.Value;
                                var logger = serviceProvider.GetService<ILogger<ConsulWorkIdCreateStrategy>>();
                                setup.UseConsulWorkIdCreateStrategy(consulClient, logger, setup.CenterId, appId);
                                break;
                            case EFlashUniqueIdGenerator4GeneratorType.RedisWorkId:
                                var cacheClient = flash.Services.BuildServiceProvider().GetService<ICacheManager>();
                                Check.Argument.IsNotNull(cacheClient, nameof(ICacheManager), "未注册Cache组件，请添加Cache配置信息");

                                setup.CenterId = centerId.Value;
                                var redisLogger = serviceProvider.GetService<ILogger<RedisWorkIdCreateStrategy>>();
                                setup.UseRedisWorkIdCreateStrategy(cacheClient, redisLogger, setup.CenterId, appId);
                                break;
                            default:
                                break;
                        }
                    });
                }
                #endregion

                #region 日志链路追踪
                var enableLoggerTracing = this.Configuration.GetSection("FlashConfiguration:LoggerTracing:Enable").Get<bool?>();
                var tracerType = this.Configuration.GetSection("FlashConfiguration:LoggerTracing:TracerType").Get<EFlashLoggerTracing4TracerType>();

                if (enableLoggerTracing.HasValue && enableLoggerTracing.Value && tracerType != EFlashLoggerTracing4TracerType.None)
                {
                    flash.AddLoggerTracing(option =>
                    {
                        switch (tracerType)
                        {
                            case EFlashLoggerTracing4TracerType.Jaeger:

                                var isOpen = this.Configuration.GetSection("FlashConfiguration:LoggerTracing:JaegerConfig:Open").Get<bool?>();
                                var agentHost = this.Configuration.GetSection("FlashConfiguration:LoggerTracing:JaegerConfig:AgentHost").Get<string>();
                                var agentPort = this.Configuration.GetSection("FlashConfiguration:LoggerTracing:JaegerConfig:AgentPort").Get<int?>();
                                var endPoint = this.Configuration.GetSection("FlashConfiguration:LoggerTracing:JaegerConfig:EndPoint").Get<string>();
                                var serivceName = this.Configuration.GetSection("FlashConfiguration:LoggerTracing:JaegerConfig:SerivceName").Get<string>();
                                var ignorePaths = this.Configuration.GetSection("FlashConfiguration:LoggerTracing:JaegerConfig:IgnorePaths").Get<List<string>>();

                                if (!string.IsNullOrEmpty(agentHost))
                                {
                                    if (!agentPort.HasValue || agentPort.Value < 0)
                                    {
                                        throw new ArgumentException("appsettings.json配置文件未设置【FlashConfiguration:LoggerTracing:JaegerConfig:AgentPort】");
                                    }
                                }
                                else if (string.IsNullOrEmpty(endPoint))
                                {
                                    throw new ArgumentException($"appsettings.json配置文件未设置【FlashConfiguration:LoggerTracing:JaegerConfig:EndPoint】");
                                }

                                if (string.IsNullOrEmpty(serivceName)) throw new ArgumentException($"appsettings.json配置文件未设置【FlashConfiguration:LoggerTracing:JaegerConfig:SerivceName】");

                                if (!isOpen.HasValue || !isOpen.Value) logger.LogWarning($"appsettings.json配置文件【FlashConfiguration:LoggerTracing:JaegerConfig:Open = {isOpen}】，暂不开启日志链路跟踪");

                                option.UseJaeger(config =>
                                {
                                    config.Open = isOpen.HasValue ? isOpen.Value : false;
                                    config.AgentHost = agentHost;
                                    config.AgentPort = agentPort.HasValue ? agentPort.Value : 5775;
                                    config.SerivceName = serivceName;
                                    config.EndPoint = endPoint;
                                }, ignorePaths);
                                break;
                            case EFlashLoggerTracing4TracerType.Skywalking:
                                break;
                        }
                    });
                }
                #endregion

                #region HealthCheck
                var enableHealthCheck = this.Configuration.GetSection("FlashConfiguration:HealthCheck:Enable").Get<bool?>();
                var healthCheckTypes = this.Configuration.GetSection("FlashConfiguration:HealthCheck:CheckType").Get<List<EFlashHealthCheck4CheckType>>();
                if (enableHealthCheck.HasValue && enableHealthCheck.Value)
                {
                    services.AddHealthChecks(check =>
                    {
                        foreach (var healthCheckType in healthCheckTypes)
                        {
                            switch (healthCheckType)
                            {
                                case EFlashHealthCheck4CheckType.Redis:
                                    var hosts = this.Configuration.GetSection("FlashConfiguration:Cache:RedisConfig:Host").Get<string>();
                                    var password = this.Configuration.GetSection("FlashConfiguration:Cache:RedisConfig:Password").Get<string>();
                                    if (string.IsNullOrEmpty(hosts) || string.IsNullOrEmpty(password)) throw new ArgumentException($"appsettings.json配置文件未设置【FlashConfiguration:Cache:RedisConfig:Host 或 FlashConfiguration:Cache:RedisConfig:Password】");

                                    foreach (var host in hosts.Split(','))
                                    {
                                        check.AddRedisCheck($"{host}", $"{host},password={password},allowAdmin=true,ssl=false,abortConnect=false,connectTimeout=5000");
                                    }
                                    break;
                                case EFlashHealthCheck4CheckType.MySql:
                                    var connectionStrings = this.Configuration.GetSection("DbConnectionString").Get<Dictionary<string, string>>();
                                    foreach (var item in connectionStrings)
                                    {
                                        check.AddMySqlCheck(item.Key, item.Value);
                                    }
                                    break;
                                case EFlashHealthCheck4CheckType.RabbitMQ:
                                    var mqOption = serviceProvider.GetService<Microsoft.Extensions.DependencyInjection.RabbitMQOption>();
                                    if (mqOption != null)
                                    {
                                        check.AddRabbitMQCheck(mqOption.HostName, setup =>
                                        {
                                            setup.WithEndPoint(mqOption.HostName, mqOption.Port)
                                                .WithAuth(mqOption.UserName, mqOption.Password)
                                                .WithExchange(mqOption.VirtualHost, mqOption.ExchangeType, mqOption.Exchange);
                                        });
                                    }
                                    break;
                            }
                        }
                    });
                }
                #endregion

            });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (this.Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            var registerResponseTracrIdService = app.ApplicationServices.GetService<IRegisterResponseTracrIdService>();
            if (registerResponseTracrIdService != null)
            {
                app.UseResponseTracrIdMiddleware();
            }

            var requestLogger = this.Configuration.GetSection("FlashConfiguration:LoggerTracing:RequestLogger").Get<bool?>();
            var responseLogger = this.Configuration.GetSection("FlashConfiguration:LoggerTracing:ResponseLogger").Get<bool?>();
            if ((requestLogger.HasValue && requestLogger.Value) || (responseLogger.HasValue && responseLogger.Value))
            {
                app.UseLoggerTracingMiddleware();
            }

            this.ConfigApplication(app, this.Env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// 配置自定义<see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services"></param>
        public abstract void ConfigServices(IServiceCollection services);

        /// <summary>
        /// 配置自定义<see cref="IApplicationBuilder"/>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public abstract void ConfigApplication(IApplicationBuilder app, IWebHostEnvironment env);
    }
}
