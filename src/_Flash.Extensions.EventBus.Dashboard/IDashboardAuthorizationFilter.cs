using System;

namespace Flash.Extensions.EventBus.Dashboard
{
    public interface IDashboardAuthorizationFilter
    {
        bool Authorize([NotNull] DashboardContext context);
    }
}
