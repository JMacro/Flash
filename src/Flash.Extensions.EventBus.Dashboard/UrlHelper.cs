﻿using System;

namespace Flash.Extensions.EventBus.Dashboard
{
    public class UrlHelper
    {
        private readonly DashboardContext _context;

        public UrlHelper([NotNull] DashboardContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _context = context;
        }

        public string To(string relativePath)
        {
            return _context.Options.PrefixPath +
                       _context.Request.PathBase
                       + relativePath
                       ;
        }

        public string Home()
        {
            return To("/");
        }

        public string JobDetails(string jobId)
        {
            return To("/jobs/details/" + jobId);
        }

        public string LinkToQueues()
        {
            return To("/jobs/enqueued");
        }

        public string Queue(string queue)
        {
            return To("/jobs/enqueued/" + queue);
        }
    }
}
