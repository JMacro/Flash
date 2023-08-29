using Flash.Extensions.Office;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Flash.Test.StartupTests
{
    public class RedisStartupTest : BaseStartupTest
    {
        public RedisStartupTest(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            //services.AddMetrics(configuration.GetSection("AppMetrics"));
            services.AddFlash(flash =>
            {
                flash.AddUniqueIdGenerator(setup =>
                {
                    setup.CenterId = 0;
                    setup.UseStaticWorkIdCreateStrategy(0);
                });

                var host = Environment.GetEnvironmentVariable("Redis_Host", EnvironmentVariableTarget.Machine);
                var password = Environment.GetEnvironmentVariable("Redis_Password", EnvironmentVariableTarget.Machine);

                flash.AddCache(cache =>
                {
                    cache.UseRedis(option =>
                    {
                        option.WithNumberOfConnections(5)
                        .WithWriteServerList(host)
                        .WithReadServerList(host)
                        .WithDb(0)
                        .WithDistributedLock(true, false)
                        .WithPassword(password)
                        .WithKeyPrefix("JMacro:Flash:Tests");
                    });
                });

                services.AddHealthChecks(check =>
                {
                    check.AddRedisCheck($"{host}", $"{host},password={password},allowAdmin=true,ssl=false,abortConnect=false,connectTimeout=5000");
                });
            });
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);
        }
    }
}
