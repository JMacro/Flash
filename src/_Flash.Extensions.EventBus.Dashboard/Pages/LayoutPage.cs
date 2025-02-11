using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.EventBus.Dashboard.Pages
{
    public partial class LayoutPage
    {
        public LayoutPage(string title)
        {
            Title = title;
        }

        public string Title { get; }
    }
}
