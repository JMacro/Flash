using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Flash.Extensions.EventBus.Dashboard.AspNetCore
{
    public class AspNetCoreDashboardMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMonitoringApi _eventBusApi;
        private readonly DashboardOptions _options;
        private readonly RouteCollection _routes;

        public AspNetCoreDashboardMiddleware(
            [NotNull] RequestDelegate next,
            [NotNull] IMonitoringApi eventBusApi,
            [NotNull] DashboardOptions options,
            [NotNull] RouteCollection routes)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            if (eventBusApi == null) throw new ArgumentNullException(nameof(eventBusApi));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (routes == null) throw new ArgumentNullException(nameof(routes));

            _next = next;
            _eventBusApi = eventBusApi;
            _options = options;
            _routes = routes;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var context = new AspNetCoreDashboardContext(_eventBusApi, _options, httpContext);
            var findResult = _routes.FindDispatcher(httpContext.Request.Path.Value);

            if (findResult == null)
            {
                await _next.Invoke(httpContext);
                return;
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var filter in _options.Authorization)
            {
                if (!filter.Authorize(context))
                {
                    httpContext.Response.StatusCode = GetUnauthorizedStatusCode(httpContext);
                    return;
                }
            }

            foreach (var filter in _options.AsyncAuthorization)
            {
                if (!await filter.AuthorizeAsync(context))
                {
                    httpContext.Response.StatusCode = GetUnauthorizedStatusCode(httpContext);
                    return;
                }
            }

            if (!_options.IgnoreAntiforgeryToken)
            {
                var antiforgery = httpContext.RequestServices.GetService<IAntiforgery>();

                if (antiforgery != null)
                {
                    var requestValid = await antiforgery.IsRequestValidAsync(httpContext);

                    if (!requestValid)
                    {
                        // Invalid or missing CSRF token
                        httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return;
                    }
                }
            }

            context.UriMatch = findResult.Item2;

            await findResult.Item1.Dispatch(context);
        }

        private static int GetUnauthorizedStatusCode(HttpContext httpContext)
        {
            return httpContext.User?.Identity?.IsAuthenticated == true
                ? (int)HttpStatusCode.Forbidden
                : (int)HttpStatusCode.Unauthorized;
        }
    }
}
