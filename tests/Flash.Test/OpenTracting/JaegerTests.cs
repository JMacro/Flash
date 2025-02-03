using Flash.Test.StartupTests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using System.Threading;

namespace Flash.Test.OpenTracting
{
    [TestFixture]
    public class JaegerTests : BaseTest<JaegerTestStartupTest>
    {
        public override void Initialize()
        {
            var webHostBuilder = new WebHostBuilder()
             .UseStartup<JaegerTestStartupTest>()
             .UseMetricsWebTracking();

            this.TestServer = new TestServer(webHostBuilder);
            this.ServiceProvider = this.TestServer.Services;
        }

        [Test]
        public void AddLog()
        {
            var response = this.TestServer.CreateRequest($"/jaeger").GetAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsNotNull(response);
        }
    }
}
