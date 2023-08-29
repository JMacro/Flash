/*
 * Licensed to the SkyAPM under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The SkyAPM licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using Flash.Extensions.Tracting;
using Flash.Extensions.Tracting.Skywalking;
using Flash.Extensions.Tracting.Skywalking.Diagnostics;
using Flash.Extensions.Tracting.Skywalking.Sampling;
using Flash.Extensions.Tracting.Skywalking.Service;
using Flash.Extensions.Tracting.Skywalking.Transport;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public class SkywalkingOption
    {
        /// <summary>
        /// 服务名
        /// </summary>
        internal string ServiceName = "";
        /// <summary>
        /// 3秒内最大采集数,小于0不限制
        /// </summary>
        internal int SamplePer3Secs = -1;
        /// <summary>
        /// 随机采集百分比,小于或等于0不限制
        /// </summary>
        internal double Percentage = -1.0;
        /// <summary>
        /// 传输轮询时间,单位秒,默认3000
        /// </summary>
        internal int Interval = 3000;
        /// <summary>
        /// 本地队列最大暂存值,默认30000
        /// </summary>
        internal int QueueSize = 30000;
        /// <summary>
        /// 传输最大数量,默认3000
        /// </summary>
        internal int BatchSize = 3000;
        /// <summary>
        /// 服务名
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public SkywalkingOption WithServiceName(string serviceName)
        {
            this.ServiceName = serviceName;
            return this;
        }
        /// <summary>
        /// 3秒内最大采集数,小于0不限制
        /// </summary>
        /// <param name="samplePer3Secs"></param>
        /// <returns></returns>
        public SkywalkingOption WithSamplePer3Secs(int samplePer3Secs)
        {
            this.SamplePer3Secs = samplePer3Secs;
            return this;
        }
        /// <summary>
        /// 随机采集百分比,小于或等于0不限制
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public SkywalkingOption WithPercentage(double percentage)
        {
            this.Percentage = percentage;
            return this;
        }
        /// <summary>
        /// 传输轮询时间,单位秒
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public SkywalkingOption WithInterval(int interval)
        {
            this.Interval = interval;
            return this;
        }
        /// <summary>
        /// 本地队列最大暂存值
        /// </summary>
        /// <param name="queueSize"></param>
        /// <returns></returns>
        public SkywalkingOption WithQueueSize(int queueSize)
        {
            this.QueueSize = queueSize;
            return this;
        }
        /// <summary>
        /// 传输最大数量
        /// </summary>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public SkywalkingOption WithBatchSize(int batchSize)
        {
            this.BatchSize = batchSize;
            return this;
        }
    }

    public static class DependencyInjectionExtersion
    {
        public static IFlashTractingBuilder UseSkywalking(this IFlashTractingBuilder builder, string serviceName)
        {
            return builder.UseSkywalking(setup =>
            {
                setup.WithServiceName(serviceName);
            });
        }


        public static IFlashTractingBuilder UseSkywalking(this IFlashTractingBuilder builder, Action<SkywalkingOption> setup)
        {
            setup = setup ?? throw new ArgumentNullException(nameof(setup));

            var option = new SkywalkingOption();
            setup(option);

            builder.Services.TryAddSingleton<SkyApmConfig>((sp) =>
            {
                var config = new SkyApmConfig();
                config.ServiceName = option.ServiceName;
                config.Sampling.Percentage = option.Percentage;
                config.Sampling.SamplePer3Secs = option.SamplePer3Secs;
                config.Transport.Interval = option.Interval;
                config.Transport.QueueSize = option.QueueSize;
                config.Transport.BatchSize = option.BatchSize;
                return config;
            });

            builder.Services.TryAddSingleton<ISegmentDispatcher, AsyncQueueSegmentDispatcher>();
            builder.Services.TryAddSingleton<IExecutionService, SegmentReportService>();
            builder.Services.TryAddSingleton<IInstrumentStartup, InstrumentStartup>();
            builder.Services.TryAddSingleton<IRuntimeEnvironment>(RuntimeEnvironment.Instance);
            builder.Services.TryAddSingleton<TracingDiagnosticProcessorObserver>();
#if NETCORE
            builder.Services.TryAddSingleton<IHostedService, InstrumentationHostedService>();
#endif
            builder.Services.AddTracing().AddSampling().AddTransport();

            return builder;
        }

        private static IServiceCollection AddTracing(this IServiceCollection services)
        {
            services.TryAddSingleton<ITracingContext, TracingContext>();
            services.TryAddSingleton<ICarrierPropagator, CarrierPropagator>();
            services.TryAddSingleton<ICarrierFormatter, BucketCarrierFormatter>();
            services.TryAddSingleton<ISegmentContextFactory, SegmentContextFactory>();
            services.TryAddSingleton<IEntrySegmentContextAccessor, EntrySegmentContextAccessor>();
            services.TryAddSingleton<ILocalSegmentContextAccessor, LocalSegmentContextAccessor>();
            services.TryAddSingleton<IExitSegmentContextAccessor, ExitSegmentContextAccessor>();
            services.TryAddSingleton<ISamplerChainBuilder, SamplerChainBuilder>();
            services.TryAddSingleton<ISegmentContextMapper, SegmentContextMapper>();
            services.TryAddSingleton<IBase64Formatter, Base64Formatter>();
            return services;
        }

        private static IServiceCollection AddSampling(this IServiceCollection services)
        {
            services.TryAddSingleton<SimpleCountSamplingInterceptor>();
            services.TryAddSingleton<ISamplingInterceptor>(p => p.GetService<SimpleCountSamplingInterceptor>());
            services.TryAddSingleton<IExecutionService>(p => p.GetService<SimpleCountSamplingInterceptor>());
            services.TryAddSingleton<ISamplingInterceptor, RandomSamplingInterceptor>();
            return services;
        }

        private static IServiceCollection AddTransport(this IServiceCollection services)
        {
            services.TryAddSingleton<ISegmentReporter, NullSegmentReporter>();
            return services;
        }
    }
}
