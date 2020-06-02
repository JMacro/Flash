using App.Metrics;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加监控插件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configurationSection"></param>
        /// <returns></returns>
        public static IServiceCollection AddMetrics(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            var metricsBuilder = App.Metrics.AppMetrics.CreateDefaultBuilder();
            services.AddSingleton<App.Metrics.IMetricsRoot>(metricsBuilder.AddReport(configurationSection).Build());
            return services;
        }

        public static IMetricsBuilder AddReport(this IMetricsBuilder metricsBuilder, IConfigurationSection configurationSection)
        {
            #region report to influxdb
            var appMetrics_Influxdb_Enable = configurationSection["Influxdb:Enable"];
            if (appMetrics_Influxdb_Enable.ToLower() == bool.TrueString.ToLower())
            {
                metricsBuilder.Configuration.Configure(options =>
                {
                    options.AddEnvTag(configurationSection["Environment"]);
                    options.Enabled = true;
                });

                metricsBuilder.Report.ToInfluxDb(options =>
                {
                    var appMetrics_influxdb_address = configurationSection["Influxdb:Address"];
                    var appMetrics_influxdb_database = configurationSection["Influxdb:Database"];
                    var appMetrics_influxdb_username = configurationSection["Influxdb:UserName"];
                    var appMetrics_influxdb_password = configurationSection["Influxdb:Password"];

                    options.HttpPolicy = new App.Metrics.Reporting.InfluxDB.Client.HttpPolicy
                    {
                        FailuresBeforeBackoff = 3,
                        BackoffPeriod = TimeSpan.FromSeconds(30),
                        Timeout = TimeSpan.FromSeconds(3)
                    };
                    options.FlushInterval = TimeSpan.FromSeconds(5);

                    options.InfluxDb = new App.Metrics.Reporting.InfluxDB.InfluxDbOptions()
                    {
                        BaseUri = new Uri(appMetrics_influxdb_address),
                        Database = appMetrics_influxdb_database,
                        UserName = appMetrics_influxdb_username,
                        Password = appMetrics_influxdb_password,
                        CreateDataBaseIfNotExists = true,
                    };
                });
            }
            else
            {
                metricsBuilder.Configuration.Configure(options =>
                {
                    options.Enabled = false;
                });
            }
            #endregion

            return metricsBuilder;
        }
    }
}
