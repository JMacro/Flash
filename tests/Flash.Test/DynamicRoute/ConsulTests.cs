using Consul;
using Flash.DynamicRoute;
using Flash.Extensions.DynamicRoute.Consul;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Test.DynamicRoute
{
    [TestFixture]
    public class ConsulTests
    {
        private ConsulClient consulClient;

        #region Setup/Teardown

        /// <summary>
        /// Code that is run before each test
        /// </summary>
        [SetUp]
        public virtual void Initialize()
        {
            consulClient = new ConsulClient(delegate (ConsulClientConfiguration obj)
            {
                obj.Address = new Uri("http://localhost:8500");
                obj.Datacenter = "dc1";
                obj.Token = "";
            });
        }

        /// <summary>
        /// Code that is run after each test
        /// </summary>
        [TearDown]
        public virtual void Cleanup()
        {
        }
        #endregion

        [Test]
        public void WhenTagExistsSuccess()
        {
            IServiceLocator serviceLocator = new ConsulServiceLocator(null, consulClient);
            var serviceEndPoints = serviceLocator.GetAsync("example", "dev").ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.True(serviceEndPoints.Any());
        }

        [Test]
        public void WhenTagNotExistsSuccess()
        {
            IServiceLocator serviceLocator = new ConsulServiceLocator(null, consulClient);
            var serviceEndPoints = serviceLocator.GetAsync("example", Guid.NewGuid().ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.True(!serviceEndPoints.Any());
        }
    }
}
