﻿using Castle.DynamicProxy;
using Flash.Core;
using Flash.Extensions.Tracting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flash.Extensions.Cache
{
    public class TracerAsyncInterceptor : AsyncInterceptorBase
    {
        private readonly ITracerFactory _tracerFactory;
        private readonly ICacheConfig _cacheConfig;

        public TracerAsyncInterceptor(ITracerFactory tracerFactory, ICacheConfig cacheConfig)
        {
            this._tracerFactory = tracerFactory;
            this._cacheConfig = cacheConfig;
        }

        protected override Task AfterProceedAsync(IInvocation invocation, bool hasAsynResult)
        {
            LoggerTracer(invocation, hasAsynResult);
            return Task.CompletedTask;
        }

        protected override void AfterProceedSync(IInvocation invocation)
        {
            LoggerTracer(invocation);
        }

        protected override void BeforeProceed(IInvocation invocation)
        {

        }

        private void LoggerTracer(IInvocation invocation, bool hasAsynResult = false)
        {
            using (var tracer = this._tracerFactory.CreateTracer($"Redis Execute({invocation.MethodInvocationTarget.Name})"))
            {
                tracer.SetComponent("StackExchange.Redis");
                tracer.SetTag("redis.key.prefix", this._cacheConfig.KeyPrefix);
                tracer.SetTag("redis.db.num", this._cacheConfig.DBNum);
                tracer.SetTag("redis.request.command", invocation.MethodInvocationTarget.Name);
                var parameters = invocation.MethodInvocationTarget.GetParameters();
                Dictionary<string, object> keyValues = new Dictionary<string, object>();
                for (int i = 0; i < parameters.Length; i++)
                {
                    keyValues.Add(parameters[i].Name, invocation.GetArgumentValue(i));
                }
                tracer.LogRequest(keyValues);
                if (hasAsynResult)
                {
                    tracer.LogResponse(this.ProceedAsyncResult);
                }
            }
        }
    }
}
