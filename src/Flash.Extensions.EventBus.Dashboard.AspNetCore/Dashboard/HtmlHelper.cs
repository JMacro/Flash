using Flash.Extensions.EventBus.Dashboard.Common;
using Flash.Extensions.EventBus.Dashboard.Pages;
using Flash.Extensions.EventBus.Dashboard.Resources;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;

namespace Flash.Extensions.EventBus.Dashboard
{
    public class HtmlHelper
    {
        private static readonly Type DisplayNameType;
        private static readonly Func<object, string> GetDisplayName;

        private readonly RazorPage _page;

        static HtmlHelper()
        {
            try
            {
                DisplayNameType = typeof(DisplayNameAttribute);
                if (DisplayNameType == null) return;

                var p = Expression.Parameter(typeof(object));
                var converted = Expression.Convert(p, DisplayNameType);

                GetDisplayName = Expression.Lambda<Func<object, string>>(Expression.Call(converted, "get_DisplayName", null), p).Compile();
            }
            catch
            {
                // Ignoring
            }
        }

        public HtmlHelper([NotNull] RazorPage page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));
            _page = page;
        }

        public NonEscapedString RenderPartial(RazorPage partialPage)
        {
            partialPage.Assign(_page);
            return new NonEscapedString(partialPage.ToString());
        }

        public NonEscapedString Raw(string value)
        {
            return new NonEscapedString(value);
        }

        public NonEscapedString LocalTime(DateTime value)
        {
            return Raw($"<span data-moment-local=\"{HtmlEncode(UtilHelper.ToTimestamp(value).ToString(CultureInfo.InvariantCulture))}\">{HtmlEncode(value.ToString(CultureInfo.CurrentUICulture))}</span>");
        }

        public string HtmlEncode(string text)
        {
            return WebUtility.HtmlEncode(text);
        }

        public string UrlEncode(string text)
        {
            return WebUtility.UrlEncode(text);
        }

        public NonEscapedString QueueLabel(string queue)
        {
            var label = queue != null
                ? $"<a href=\"{HtmlEncode(_page.Url.Queue(UrlEncode(queue)))}\">{HtmlEncode(queue)}</a>"
                : $"<span class=\"label label-danger\"><i>{HtmlEncode(Strings.Common_Unknown)}</i></span>";

            return new NonEscapedString(label);
        }

        public NonEscapedString InlineMetric([NotNull] DashboardMetric metric)
        {
            if (metric == null) throw new ArgumentNullException(nameof(metric));
            return RenderPartial(new InlineMetric(metric));
        }

        public NonEscapedString QueueSidebar()
        {
            return RenderPartial(new SidebarMenu(QueueSidebarMenu.Items));
        }

        public NonEscapedString PerPageSelector([NotNull] Pager pager)
        {
            if (pager == null) throw new ArgumentNullException(nameof(pager));
            return RenderPartial(new PerPageSelector(pager));
        }

        public NonEscapedString Paginator([NotNull] Pager pager)
        {
            if (pager == null) throw new ArgumentNullException(nameof(pager));
            return RenderPartial(new Paginator(pager));
        }
    }
}
