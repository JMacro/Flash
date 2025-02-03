using System;
using System.Collections.Generic;

namespace Flash.Extensions.EventBus.Dashboard.Pages
{
    partial class SidebarMenu
    {
        public SidebarMenu([NotNull] IEnumerable<Func<RazorPage, MenuItem>> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            Items = items;
        }

        public IEnumerable<Func<RazorPage, MenuItem>> Items { get; }
    }
}
