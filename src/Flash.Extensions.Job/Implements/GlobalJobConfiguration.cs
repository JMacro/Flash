using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.Job
{
    public class GlobalJobConfiguration : IGlobalJobConfiguration
    {
        public GlobalJobConfiguration()
        {
        }

        public string DashboardPath { get; set; }
        public string SectionName { get; set; } = "CornJobScheduler";
    }
}
