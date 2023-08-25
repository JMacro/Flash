using Flash.Extensions.Cache;
using Flash.Extensions.UidGenerator;
using Flash.Test.StartupTests;
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
    public class UniqueIdGeneratorTests : BaseTest<UniqueIdGeneratorStartupTest>
    {
        [Test]
        public void NewIdTest()
        {
            var uniqueIdGenerator = this.ServiceProvider.GetService<IUniqueIdGenerator>();
            var ids = new List<long>();
            for (var i = 0; i < 10000; i++)
            {
                ids.Add(uniqueIdGenerator.NewId());
            }
            Assert.IsFalse(ids.GroupBy(x => x).Select(x => new { x.Key, Count = x.Count() }).Any(p => p.Count > 1));
        }
    }
}
