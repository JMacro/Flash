using Flash.Extensions.Resilience.Http;
using Flash.Test.StartupTests;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ubiety.Dns.Core;

namespace Flash.Test
{
    [TestFixture]
    public class ResilientHttpTests : BaseTest<ResilientHttpStartupTest>
    {
        private IHttpClient _httpClient;

        public override void Initialize()
        {
            base.Initialize();
            this._httpClient = this.ServiceProvider.GetService<IHttpClient>();
        }

        [Test]
        public void GetTest()
        {
            Assert.IsNotNull(this._httpClient);

            var result = this._httpClient.GetStringAsync("http://www.baidu.com").ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsNotNull(result);
        }

        [Test]
        public void PostTest()
        {
            Assert.IsNotNull(this._httpClient);

            var result = this._httpClient.PostAsync("https://ug.baidu.com/mcp/pc/pcsearch", new { }).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            Assert.DoesNotThrow(new TestDelegate(() =>
            {
                result.ReadAsObjectAsync<dynamic>().ConfigureAwait(false).GetAwaiter().GetResult();
            }));
        }
    }
}
