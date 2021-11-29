using System;
using System.Threading.Tasks;

namespace Flash.Extensions.EventBus.Dashboard
{
    public interface IDashboardDispatcher
    {
        Task Dispatch([NotNull] DashboardContext context);
    }
}
