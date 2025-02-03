using Flash.Extensions;
using Flash.Extensions.EventBus;
using Flash.Extensions.EventBus.Dashboard;
using Flash.Extensions.EventBus.Dashboard.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
        public static IApplicationBuilder UseEventBusDashboard(
            [NotNull] this IApplicationBuilder app,
            [NotNull] string pathMatch = "/eventBus",
            [CanBeNull] DashboardOptions options = null)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (pathMatch == null) throw new ArgumentNullException(nameof(pathMatch));

            var services = app.ApplicationServices;

            options = options ?? services.GetService<DashboardOptions>() ?? new DashboardOptions();

            var routes = app.ApplicationServices.GetRequiredService<RouteCollection>();

            app.Map(new PathString(pathMatch), x => x.UseMiddleware<AspNetCoreDashboardMiddleware>(options, routes));

            return app;
        }

        public static IEventBusHostBuilder UseRabbitMQ(this IEventBusHostBuilder hostBuilder)
        {
            hostBuilder.Services.TryAddSingleton(_ => DashboardRoutes.Routes);
            return hostBuilder;
        }
    }
}
