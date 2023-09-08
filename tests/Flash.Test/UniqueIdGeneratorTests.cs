using Consul;
using Flash.Extensions.Cache;
using Flash.Extensions.UidGenerator;
using Flash.Extensions.UidGenerator.Consul;
using Flash.Extensions.UidGenerator.WorkIdCreateStrategy;
using Flash.Test.StartupTests;
using Google.Protobuf.WellKnownTypes;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Test
{
    [TestFixture]
    public class UniqueIdGeneratorTests : BaseTest
    {
        private int centerId = 0;

        #region Setup/Teardown

        /// <summary>
        /// Code that is run before each test
        /// </summary>
        [SetUp]
        public void Initialize()
        {
        }

        /// <summary>
        /// Code that is run after each test
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
        }
        #endregion

        [Test]
        public void StaticWorkIdTest()
        {
            var workIdCreateStrategy = new StaticWorkIdCreateStrategy(0);
            var uniqueIdGenerator = new SnowflakeUniqueIdGenerator(workIdCreateStrategy.GetWorkId(), centerId);

            var ids = new List<long>();
            for (var i = 0; i < 10000; i++)
            {
                ids.Add(uniqueIdGenerator.NewId());
            }
            Assert.IsFalse(ids.GroupBy(x => x).Select(x => new { x.Key, Count = x.Count() }).Any(p => p.Count > 1));
        }

        [Test]
        public void ConsulWorkIdTest()
        {
            var consulClient = new ConsulClient(delegate (ConsulClientConfiguration obj)
            {
                obj.Address = new Uri("http://localhost:8500");
                obj.Datacenter = "dc1";
                obj.Token = "";
            });
            var workIdCreateStrategy = new ConsulWorkIdCreateStrategy(consulClient, null, centerId, "example", TimeSpan.FromSeconds(300), TimeSpan.FromSeconds(5));
            var workId = workIdCreateStrategy.GetWorkId();

            var uniqueIdGenerator = new SnowflakeUniqueIdGenerator(workId, centerId);
            var ids = new List<long>();
            for (var i = 0; i < 10000; i++)
            {
                ids.Add(uniqueIdGenerator.NewId());
            }
            Assert.IsFalse(ids.GroupBy(x => x).Select(x => new { x.Key, Count = x.Count() }).Any(p => p.Count > 1));
        }
    }
}
