using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Flash.Extensions.EventBus.Dashboard.AspNetCore
{
    public sealed class AspNetCoreDashboardContext : DashboardContext
    {
        public AspNetCoreDashboardContext(
            [NotNull] IMonitoringApi eventBusApi,
            [NotNull] DashboardOptions options,
            [NotNull] HttpContext httpContext)
            : base(eventBusApi, options)
        {
            HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            Request = new AspNetCoreDashboardRequest(httpContext);
            Response = new AspNetCoreDashboardResponse(httpContext);

            if (!options.IgnoreAntiforgeryToken)
            {
                var antiforgery = HttpContext.RequestServices.GetService<IAntiforgery>();
                var tokenSet = antiforgery?.GetAndStoreTokens(HttpContext);

                if (tokenSet != null)
                {
                    AntiforgeryHeader = tokenSet.HeaderName;
                    AntiforgeryToken = tokenSet.RequestToken;
                }
            }
        }

        public HttpContext HttpContext { get; }
    }
}
