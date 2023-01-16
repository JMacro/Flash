using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Flash.Extensions.EventBus.Dashboard
{
    class RazorPageDispatcher : IDashboardDispatcher
    {
        private readonly Func<Match, RazorPage> _pageFunc;

        public RazorPageDispatcher(Func<Match, RazorPage> pageFunc)
        {
            _pageFunc = pageFunc;
        }

        public Task Dispatch(DashboardContext context)
        {
            context.Response.ContentType = "text/html";

            var page = _pageFunc(context.UriMatch);
            page.Assign(context);

            return context.Response.WriteAsync(page.ToString());
        }
    }
}
