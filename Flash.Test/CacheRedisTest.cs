using Autofac;
using Flash.Extersions.Cache;
using Flash.Extersions.DistributedLock;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

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
            this._cacheManager.StringSet<int>("123",123);
            var d = this._cacheManager.StringGet<int>("123");

            var ddd = this._distributedLock.Enter("122", "AAAAA", TimeSpan.FromSeconds(60));
        }
    }
}
