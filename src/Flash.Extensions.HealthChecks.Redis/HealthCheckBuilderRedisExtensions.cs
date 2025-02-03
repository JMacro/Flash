using Flash.LoadBalancer;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Flash.Extensions.HealthChecks
{
    public static class HealthCheckBuilderRedisExtensions
    {
        public static HealthCheckBuilder AddRedisCheck(this HealthCheckBuilder builder, string name, string connectionString)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);
            return AddRedisCheck(builder, name, builder.DefaultCacheDuration, connectionString);
        }

        public static HealthCheckBuilder AddRedisCheck(this HealthCheckBuilder builder, string name, ConcurrentDictionary<string, ConnectionMultiplexer> connectionCache)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);
            return AddRedisCheck(builder, name, builder.DefaultCacheDuration, connectionCache);
        }

        public static HealthCheckBuilder AddRedisCheck(this HealthCheckBuilder builder, string name, TimeSpan cacheDuration, string connectionString)
        {
            builder.AddCheck($"RedisCheck({name})", () =>
            {
                try
                {
                    using (ConnectionMultiplexer connect = ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(connectionString)))
                    {
                        if (connect.IsConnected)
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

        public static HealthCheckBuilder AddRedisCheck(this HealthCheckBuilder builder, string name, TimeSpan cacheDuration, ConcurrentDictionary<string, ConnectionMultiplexer> connectionCache)
        {
            builder.AddCheck($"RedisCheck({name})", () =>
            {
                try
                {
                    var factory = new DefaultLoadBalancerFactory<ConnectionMultiplexer>();
                    var loadBalancer = factory.Resolve(() =>
                    {
                        return connectionCache.Values.ToList();
                    });

                    var connect = loadBalancer.Resolve();
                    if (connect.IsConnected)
                    {
                        return HealthCheckResult.Healthy($"Healthy");
                    }
                    return HealthCheckResult.Unhealthy($"Unhealthy");
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy($"{ex.GetType().FullName}");
                }
            }, cacheDuration);

            return builder;
        }
    }
}
