using Flash.Extensions.EventBus.Dashboard.Resources;
using System;
using System.Collections.Generic;

namespace Flash.Extensions.EventBus.Dashboard
{
    public static class NavigationMenu
    {
        public static readonly List<Func<RazorPage, MenuItem>> Items = new List<Func<RazorPage, MenuItem>>();

        static NavigationMenu()
        {
            Items.Add(page => new MenuItem(Strings.NavigationMenu_Queues, page.Url.To("/queue/normal"))
            {
                Active = page.RequestPath.StartsWith("/queue/normal")
            });

            //Items.Add(page => new MenuItem(Strings.NavigationMenu_Retries, page.Url.To("/retries"))
            //{
            //    Active = page.RequestPath.StartsWith("/retries")
            //});

            //Items.Add(page => new MenuItem(Strings.NavigationMenu_RecurringJobs, page.Url.To("/recurring"))
            //{
            //    Active = page.RequestPath.StartsWith("/recurring")
            //});

            //Items.Add(page => new MenuItem(Strings.NavigationMenu_Servers, page.Url.To("/servers"))
            //{
            //    Active = page.RequestPath.Equals("/servers")
            //});
        }
    }
}
