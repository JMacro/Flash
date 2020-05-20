using System;
using System.Collections.Generic;
using Flash.Extersions.Cache.Redis;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        const string NAME = "redis";

        public static IHealthChecksBuilder AddRedis(this IHealthChecksBuilder builder, string redisConnectionString, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default)
        {
            return builder.Add(new HealthCheckRegistration(
               name ?? NAME,
               sp => new HealthCheck(redisConnectionString),
               failureStatus,
               tags));
        }
    }
}
