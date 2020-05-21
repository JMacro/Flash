using Autofac;
using Flash.Extersions.Cache;
using Flash.Extersions.Cache.Redis;
using Flash.Extersions.HealthChecks.Redis;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test
{
    [TestClass]
    public class CacheRedisTest : BaseTest
    {
        private readonly ICacheManager _cacheManager;
        private readonly IDistributedLock _distributedLock;

        public CacheRedisTest() : base()
        {
            this._cacheManager = this.container.Resolve<ICacheManager>();
            this._distributedLock = this.container.Resolve<IDistributedLock>();
        }

        [TestMethod]
        public void StringSet()
        {
            this._cacheManager.StringSet<int>("123", 123);
            var d = this._cacheManager.StringGet<int>("123");

            var ddd = this._distributedLock.Enter("122", "AAAAA", TimeSpan.FromSeconds(60));

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

           
        }

        [TestMethod]
        public async Task HealthCheck()
        {
            var webHostBuilder = new WebHostBuilder()
             .ConfigureServices(services =>
             {
                 services.AddFlash(setup =>
                 {
                     setup.AddCache(cache =>
                     {
                         cache.AddRedis(option =>
                         {
                             option.WithDb(10);
                             option.WithKeyPrefix("SystemClassName:TypeClassName");
                             option.WithPassword("tY7cRu9HG_jyDw2r");
                             option.WithReadServerList("192.168.109.237:63100");
                             option.WithWriteServerList("192.168.109.237:63100");
                             option.WithSsl(false);
                             option.WithHealthCheck(true);
                             option.WithDistributedLock(true);
                         });
                     });
                 });
             })
             .Configure(app =>
             {
                 app.UseHealthChecks("/health", new HealthCheckOptions()
                 {
                     Predicate = r => r.Tags.Contains("redis")
                 });
             });


            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest($"/health").GetAsync();

            var df = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
