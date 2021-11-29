using System;
using System.Threading.Tasks;

namespace Flash.Extensions.EventBus.Dashboard
{
    public interface IDashboardAsyncAuthorizationFilter
    {
        Task<bool> AuthorizeAsync([NotNull] DashboardContext context);
    }
}
