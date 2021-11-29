using Flash.Extensions.EventBus.Dashboard.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.EventBus.Dashboard
{
    public static class QueueSidebarMenu
    {
        public static readonly List<Func<RazorPage, MenuItem>> Items = new List<Func<RazorPage, MenuItem>>();

        static QueueSidebarMenu()
        {
            Items.Add(page => new MenuItem(Strings.QueueSidebarMenu_Normal, page.Url.To("/queue/normal"))
            {
                Active = page.RequestPath.StartsWith("/queue/normal"),
                Metric = DashboardMetrics.NormalCount
            });
            Items.Add(page => new MenuItem(Strings.QueueSidebarMenu_Retry, page.Url.To("/queue/retry"))
            {
                Active = page.RequestPath.StartsWith("/queue/retry"),
                Metric = DashboardMetrics.RetryCount
            });
            Items.Add(page => new MenuItem(Strings.QueueSidebarMenu_Failed, page.Url.To("/queue/failed"))
            {
                Active = page.RequestPath.StartsWith("/queue/failed"),
                Metric = DashboardMetrics.FailedCount
            });
        }
    }
}
