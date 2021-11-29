using Flash.Extensions.EventBus.Dashboard.Pages;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Flash.Extensions.EventBus.Dashboard
{
    public static class DashboardRoutes
    {
        private static readonly string[] Javascripts =
        {
            "jquery-3.6.0.min.js",
            "bootstrap.min.js",
            "moment-with-locales.min.js",
            "Chart.min.js",
            "chartjs-plugin-streaming.min.js"
        };

        private static readonly string[] Stylesheets =
        {
            "bootstrap.min.css",
            "Chart.min.css",
            "hangfire.css"
        };

        static DashboardRoutes()
        {
            Routes = new RouteCollection();
            Routes.AddRazorPage("/", x => new HomePage());


            #region Embedded static content

            Routes.Add("/js[0-9]+", new CombinedResourceDispatcher(
                "application/javascript",
                GetExecutingAssembly(),
                GetContentFolderNamespace("js"),
                Javascripts));

            Routes.Add("/css[0-9]+", new CombinedResourceDispatcher(
                "text/css",
                GetExecutingAssembly(),
                GetContentFolderNamespace("css"),
                Stylesheets));

            Routes.Add("/fonts/glyphicons-halflings-regular/eot", new EmbeddedResourceDispatcher(
                "application/vnd.ms-fontobject",
                GetExecutingAssembly(),
                GetContentResourceName("fonts", "glyphicons-halflings-regular.eot")));

            Routes.Add("/fonts/glyphicons-halflings-regular/svg", new EmbeddedResourceDispatcher(
                "image/svg+xml",
                GetExecutingAssembly(),
                GetContentResourceName("fonts", "glyphicons-halflings-regular.svg")));

            Routes.Add("/fonts/glyphicons-halflings-regular/ttf", new EmbeddedResourceDispatcher(
                "application/octet-stream",
                GetExecutingAssembly(),
                GetContentResourceName("fonts", "glyphicons-halflings-regular.ttf")));

            Routes.Add("/fonts/glyphicons-halflings-regular/woff", new EmbeddedResourceDispatcher(
                "font/woff",
                GetExecutingAssembly(),
                GetContentResourceName("fonts", "glyphicons-halflings-regular.woff")));

            Routes.Add("/fonts/glyphicons-halflings-regular/woff2", new EmbeddedResourceDispatcher(
                "font/woff2",
                GetExecutingAssembly(),
                GetContentResourceName("fonts", "glyphicons-halflings-regular.woff2")));

            #endregion

            #region Razor pages and commands

            //Routes.AddRazorPage("/servers", x => new ServersPage());
            Routes.AddRazorPage("/queue/normal", x => new NormalQueuePage());
            Routes.AddRazorPage("/queue/failed", x => new FailedQueuePage());
            Routes.AddRazorPage("/queue/retry", x => new RetryQueuePage());
            Routes.AddRazorPage("/queue/message/(?<Queue>.+)", x => new QueueMessagePage(x.Groups["Queue"].Value));

            #endregion
        }

        public static RouteCollection Routes { get; }

        internal static string GetContentFolderNamespace(string contentFolder)
        {
            return $"{typeof(DashboardRoutes).Namespace}.Content.{contentFolder}";
        }

        internal static string GetContentResourceName(string contentFolder, string resourceName)
        {
            return $"{GetContentFolderNamespace(contentFolder)}.{resourceName}";
        }

        private static Assembly GetExecutingAssembly()
        {
            return typeof(DashboardRoutes).GetTypeInfo().Assembly;
        }
    }
}
