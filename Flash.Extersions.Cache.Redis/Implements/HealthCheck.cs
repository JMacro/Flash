using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flash.LoadBalancer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Flash.Extersions.Cache.Redis
{
    public class HealthCheck : IHealthCheck
    {
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> _connections = ConnectionHelp.GetConnections();
        private readonly string _redisConnectionString;
        private readonly DefaultLoadBalancerFactory<ConnectionMultiplexer> _factory;
        private readonly ILoadBalancer<ConnectionMultiplexer> _loadBalancer;

        public HealthCheck()
        {
            _factory = new DefaultLoadBalancerFactory<ConnectionMultiplexer>();
            _loadBalancer = _factory.Resolve(() =>
            {
                return ConnectionHelp.GetConnections().Values.ToList();
            });
        }

        public HealthCheck(string redisConnectionString) : this()
        {
            _redisConnectionString = redisConnectionString ?? throw new ArgumentNullException(nameof(redisConnectionString));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                ConnectionMultiplexer connection;
                if (!string.IsNullOrWhiteSpace(_redisConnectionString))
                {
                    connection = ConnectionHelp.CreateConnect(_redisConnectionString);
                }

                connection = _loadBalancer.Resolve();

                await connection.GetDatabase().PingAsync();

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
