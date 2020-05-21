﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flash.Extersions.HealthChecks.Redis
{
    public static class HealthCheckBuilderRedisExtensions
    {
        public static HealthCheckBuilder AddRedisCheck(this HealthCheckBuilder builder, string name, string connectionString)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);
            return AddRedisCheck(builder, name, builder.DefaultCacheDuration, connectionString);
        }

        public static HealthCheckBuilder AddRedisCheck(this HealthCheckBuilder builder, string name, TimeSpan cacheDuration, string connectionString)
        {
            builder.AddCheck($"RedisCheck({name})", () =>
            {
                try
                {
                    using (ConnectionMultiplexer connect = ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(connectionString)))
                    {
                        var response = connect.GetStatus();

                        if (response != null && response.Any())
                        {
                            return HealthCheckResult.Healthy($"Healthy");
                        }
                        return HealthCheckResult.Unhealthy($"Unhealthy");
                    }
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy($"{ex.GetType().FullName}");
                }
            }, cacheDuration);

            return builder;
        }

        //public static HealthCheckBuilder AddRedisCheck(this HealthCheckBuilder builder, string name, TimeSpan cacheDuration, string connectionString)
        //{
        //    builder.AddCheck($"RedisCheck({name})", () =>
        //    {
        //        try
        //        {
        //            using (ConnectionMultiplexer connect = ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(connectionString)))
        //            {
        //                var response = connect.GetStatus();

        //                if (response != null && response.Any())
        //                {
        //                    return HealthCheckResult.Healthy($"Healthy");
        //                }
        //                return HealthCheckResult.Unhealthy($"Unhealthy");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return HealthCheckResult.Unhealthy($"{ex.GetType().FullName}");
        //        }
        //    }, cacheDuration);

        //    return builder;
        //}
    }
}
