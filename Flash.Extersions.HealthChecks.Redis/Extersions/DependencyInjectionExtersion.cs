using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Flash.Extersions.HealthChecks.Redis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        const string NAME = "redis";

        public static IHealthChecksBuilder AddRedis(this IHealthChecksBuilder builder, string redisConnectionString, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default)
        {
            return builder.Add(new HealthCheckRegistration(name ?? NAME, sp => new RedisHealthCheck(redisConnectionString), failureStatus, tags));
        }

        public static IHealthChecksBuilder AddRedis(this IHealthChecksBuilder builder, ConcurrentDictionary<string, ConnectionMultiplexer> connections, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default)
        {
            return builder.Add(new HealthCheckRegistration(name ?? NAME, sp => new RedisHealthCheck(connections), failureStatus, tags));
        }
    }
}
