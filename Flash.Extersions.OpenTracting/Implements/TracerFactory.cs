using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extersions.OpenTracting
{
    public class TracerFactory : ITracerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public TracerFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public ITracer CreateTracer(string tracerName)
        {
            var tracer = this._serviceProvider.GetService(typeof(ITracer)) as ITracer;
            if (tracer == null)
            {
                tracer = new DefaultTracer();
            }
            tracer.SetTracerName(tracerName);
            return tracer;
        }
    }
}
