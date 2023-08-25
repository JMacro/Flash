using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using System;

namespace Flash.Test
{
    public class BaseTest
    {
        public IServiceProvider ServiceProvider { get; internal set; }
        public TestServer TestServer { get; internal set; }


        public BaseTest()
        {
        }
    }

    public abstract class BaseTest<TStartupType> : BaseTest where TStartupType : class
    {
        #region Setup/Teardown

        /// <summary>
        /// Code that is run before each test
        /// </summary>
        [SetUp]
        public virtual void Initialize()
        {
            var webHostBuilder = new WebHostBuilder()
             .UseStartup<TStartupType>();

            this.TestServer = new TestServer(webHostBuilder);
            this.ServiceProvider = this.TestServer.Services;
        }

        /// <summary>
        /// Code that is run after each test
        /// </summary>
        [TearDown]
        public virtual void Cleanup()
        {
        }
        #endregion
    }
}
