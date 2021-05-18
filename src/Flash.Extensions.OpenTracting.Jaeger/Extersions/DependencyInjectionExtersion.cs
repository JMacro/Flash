using Flash.Extensions.OpenTracting;
using Flash.Extensions.OpenTracting.Jaeger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加Jaeger链路追踪，实例对象ITracer
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configurationSection"></param>
        /// <param name="openTracingBuilder"></param>
        /// <returns></returns>
        public static IFlashTractingBuilder AddJaeger(this IFlashTractingBuilder builder, IConfigurationSection configurationSection, Action<IOpenTracingBuilder> openTracingBuilder = null)
        {
            builder.Services.AddTransient<TracingConfiguration>(sp =>
            {
                var config = configurationSection.Get<TracingConfiguration>();
                if (config == null)
                {
                    config = new TracingConfiguration() { Open = false };
                }
                return config;
            });
            AddJaeger(builder.Services, openTracingBuilder);
            return builder;
        }

        /// <summary>
        /// 添加Jaeger链路追踪，实例对象ITracer
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="action"></param>
        /// <param name="openTracingBuilder"></param>
        /// <returns></returns>
        public static IFlashTractingBuilder AddJaeger(this IFlashTractingBuilder builder, Action<TracingConfiguration> action, Action<IOpenTracingBuilder> openTracingBuilder = null)
        {
            var config = new TracingConfiguration() { Open = false };
            action = action ?? throw new ArgumentNullException(nameof(action));
            action(config);

            builder.Services.AddTransient<TracingConfiguration>(sp =>
            {
                return config;
            });
            AddJaeger(builder.Services, openTracingBuilder);
            return builder;
        }

        /// <summary>
        /// 添加Jaeger链路追踪，实例对象ITracer
        /// </summary>
        /// <param name="services"></param>
        /// <param name="openTracingBuilder"></param>
        /// <returns></returns>
        public static IServiceCollection AddJaeger(this IServiceCollection services, Action<IOpenTracingBuilder> openTracingBuilder = null)
        {
            if (openTracingBuilder == null)
            {
                openTracingBuilder = builder =>
                {
                    builder.AddCoreFx();
                    builder.AddAspNetCore();
                    builder.AddEntityFrameworkCore();
                    builder.AddLoggerProvider();
                    builder.ConfigureGenericDiagnostics(options =>
                    {

                    });
                    builder.ConfigureAspNetCore(options =>
                    {
                        options.Hosting.OperationNameResolver = (context) =>
                        {
                            return context.Request.Path.ToUriComponent();
                        };
                        options.Hosting.IgnorePatterns.Add(a =>
                        {
                            return false;
                        });
                    });
                };
            }

            services.AddOpenTracing(openTracingBuilder);
            services.AddSingleton(serviceProvider =>
            {
                var config = serviceProvider.GetService<TracingConfiguration>();
                var serviceName = config.SerivceName ?? serviceProvider.GetRequiredService<IHostingEnvironment>().ApplicationName;
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var endPoint = config.EndPoint;
                var senderConfiguration = new Jaeger.Configuration.SenderConfiguration(loggerFactory);

                if (!string.IsNullOrEmpty(config.AgentHost))
                {
                    senderConfiguration
                        .WithAgentHost(config.AgentHost)
                        .WithAgentPort(config.AgentPort);
                }
                else
                {
                    senderConfiguration.WithEndpoint(endPoint);
                }


                var samplerConfiguration = new Jaeger.Configuration.SamplerConfiguration(loggerFactory)
                    .WithType(config.SamplerType);

                var reporterConfiguration = new Jaeger.Configuration.ReporterConfiguration(loggerFactory)
                    .WithFlushInterval(TimeSpan.FromSeconds(config.FlushIntervalSeconds))
                    .WithLogSpans(config.LogSpans)
                    .WithSender(senderConfiguration);


                OpenTracing.ITracer tracer = null;
                if (config.Open)
                {
                    tracer = new Jaeger.Configuration(serviceName, loggerFactory)
                       .WithSampler(samplerConfiguration)
                       .WithReporter(reporterConfiguration)
                       .GetTracer();
                }
                else
                {
                    tracer = new Jaeger.Tracer.Builder(serviceName)
                        .WithSampler(new Jaeger.Samplers.RateLimitingSampler(0))
                        .WithReporter(new Jaeger.Reporters.NoopReporter()).Build();
                }


                if (!GlobalTracer.IsRegistered())
                {
                    GlobalTracer.Register(tracer);
                }

                return tracer;
            });
            return services;
        }
    }
}
