﻿namespace Flash.Extensions.EventBus.Dashboard
{
    public class Metric
    {
        public Metric(string value)
        {
            Value = value;
        }

        public Metric(long value)
        {
            Value = value.ToString("N0");
            IntValue = value;
        }

        public string Value { get; }
        public long IntValue { get; set; }
        public MetricStyle Style { get; set; }
        public bool Highlighted { get; set; }
        public string Title { get; set; }
    }

    public enum MetricStyle
    {
        Default,
        Info,
        Success,
        Warning,
        Danger,
    }

    internal static class MetricStyleExtensions
    {
        public static string ToClassName(this MetricStyle style)
        {
            switch (style)
            {
                case MetricStyle.Default: return "metric-default";
                case MetricStyle.Info: return "metric-info";
                case MetricStyle.Success: return "metric-success";
                case MetricStyle.Warning: return "metric-warning";
                case MetricStyle.Danger: return "metric-danger";
                default: return "metric-null";
            }
        }
    }
}
