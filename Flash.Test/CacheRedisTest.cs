using Autofac;
using Flash.Extersions.Cache;
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
        public CacheRedisTest() : base()
        {
            this._cacheManager = this.container.Resolve<ICacheManager>();
        }

        [TestMethod]
        public void StringSet()
        {
            this._cacheManager.StringSet<int>("123",123);
            var d = this._cacheManager.StringGet<int>("123");
        }
    }
}
