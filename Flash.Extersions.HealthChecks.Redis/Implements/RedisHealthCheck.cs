using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flash.LoadBalancer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Flash.Extersions.HealthChecks.Redis
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly ConcurrentDictionary<string, ConnectionMultiplexer> _connections;
        private readonly string _redisConnectionString;
        private readonly DefaultLoadBalancerFactory<ConnectionMultiplexer> _factory;
        private readonly ILoadBalancer<ConnectionMultiplexer> _loadBalancer;

        public RedisHealthCheck(ConcurrentDictionary<string, ConnectionMultiplexer> connections)
        {
            _connections = connections;
            _factory = new DefaultLoadBalancerFactory<ConnectionMultiplexer>();
            _loadBalancer = _factory.Resolve(() =>
            {
                return _connections.Values.ToList();
            });
        }

        public RedisHealthCheck(string redisConnectionString) : this(new ConcurrentDictionary<string, ConnectionMultiplexer>())
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
                    if (!_connections.TryGetValue(_redisConnectionString, out connection))
                    {
                        connection = await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);
                        if (!_connections.TryAdd(_redisConnectionString, connection))
                        {
                            connection.Dispose();
                            connection = _connections[_redisConnectionString];
                        }
                    }
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
