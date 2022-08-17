using System;
using System.Collections.Generic;

namespace Flash.Extensions.EventBus.Dashboard
{
    public class DashboardOptions
    {
        private static readonly IDashboardAuthorizationFilter[] DefaultAuthorization =
            new[] { new LocalRequestsOnlyAuthorizationFilter() };

        private IEnumerable<IDashboardAsyncAuthorizationFilter> _asyncAuthorization;

        public DashboardOptions()
        {
            AppPath = "/";
            PrefixPath = string.Empty;
            _asyncAuthorization = new IDashboardAsyncAuthorizationFilter[0];
            Authorization = DefaultAuthorization;
            IsReadOnlyFunc = _ => false;
            StatsPollingInterval = 2000;
            DisplayStorageConnectionString = true;
            DashboardTitle = "EventBus Dashboard";
        }

        /// <summary>
        /// The path for the Back To Site link. Set to <see langword="null" /> in order to hide the Back To Site link.
        /// </summary>
        public string AppPath { get; set; }

        /// <summary>
        /// The path for the first url prefix link, eg. set "/admin", then url is "{domain}/{PrefixPath}/{hangfire}"
        /// </summary>
        public string PrefixPath { get; set; }

        public IEnumerable<IDashboardAuthorizationFilter> Authorization { get; set; }

        public IEnumerable<IDashboardAsyncAuthorizationFilter> AsyncAuthorization
        {
            get => _asyncAuthorization;
            set
            {
                _asyncAuthorization = value;

                if (ReferenceEquals(Authorization, DefaultAuthorization))
                {
                    Authorization = new IDashboardAuthorizationFilter[0];
                }
            }
        }

        public Func<DashboardContext, bool> IsReadOnlyFunc { get; set; }

        /// <summary>
        /// The interval the /stats endpoint should be polled with.
        /// </summary>
        public int StatsPollingInterval { get; set; }

        public bool DisplayStorageConnectionString { get; set; }

        /// <summary>
        /// The Title displayed on the dashboard, optionally modify to describe this dashboards purpose.
        /// </summary>
        public string DashboardTitle { get; set; }

        public bool IgnoreAntiforgeryToken { get; set; }
    }
}
