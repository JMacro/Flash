using Microsoft.Extensions.Logging;
using System;

namespace Flash.Extensions.OpenTracting
{
    public class TracerFactory : ITracerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public TracerFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 创建链路追踪器，默认返回系统默认链路追踪器
        /// </summary>
        /// <param name="tracerName">追踪器名称</param>
        /// <returns></returns>
        public ITracer CreateTracer(string tracerName)
        {
            var tracer = this._serviceProvider.GetService(typeof(ITracer)) as ITracer;
            if (tracer == null)
            {
                tracer = new DefaultTracer(this._serviceProvider.GetService(typeof(ILogger<DefaultTracer>)) as ILogger<DefaultTracer>);
            }
            tracer.SetTracerName(tracerName);
            return tracer;
        }
    }
}
