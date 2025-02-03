using Flash.AspNetCore.Middlewares;
using Flash.Extensions.OpenTracting;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
#if NETCOREAPP
        /// <summary>
        /// 记录请求响应数据到OpenTracing
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLoggerTracingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<LoggerTracingMiddleware>();
            return app;
        }
    }
#endif
}
