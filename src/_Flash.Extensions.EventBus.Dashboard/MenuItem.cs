using System;
using System.Collections.Generic;
using System.Linq;

namespace Flash.Extensions.EventBus.Dashboard
{
    public class MenuItem
    {
        public MenuItem(string text, string url)
        {
            Text = text;
            Url = url;
        }

        public string Text { get; }
        public string Url { get; }

        public bool Active { get; set; }
    }
}
