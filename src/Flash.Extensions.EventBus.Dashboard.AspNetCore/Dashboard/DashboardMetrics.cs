namespace Flash.Extensions.EventBus.Dashboard
{
    public static class DashboardMetrics
    {
        public static readonly DashboardMetric FailedCount = new DashboardMetric(
            "failed:count",
            "Metrics_FailedJobs",
            page =>
            {
                return new Metric(page.Monitor.GetFailedCount())
                {
                    IntValue = 1,
                    Style = MetricStyle.Danger,
                    Highlighted = true
                };
            });

        public static readonly DashboardMetric RetryCount = new DashboardMetric(
            "awaiting:count",
            "Metrics_AwaitingCount",
            page =>
            {
                long awaitingCount = page.Monitor.GetRetryCount();
                return new Metric(awaitingCount)
                {
                    Style = awaitingCount > 0 ? MetricStyle.Info : MetricStyle.Default
                };
            });

        public static readonly DashboardMetric NormalCount = new DashboardMetric(
            "awaiting:count",
            "Metrics_AwaitingCount",
            page =>
            {
                long awaitingCount = page.Monitor.GetNormalCount();
                return new Metric(awaitingCount)
                {
                    Style = awaitingCount > 0 ? MetricStyle.Info : MetricStyle.Default
                };
            });
    }
}
