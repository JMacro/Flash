using Flash.Test.StartupTests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

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
        #region OneTimeSetUp/Teardown

        /// <summary>
        /// Code that is run before each test
        /// </summary>
        [SetUp]
        public virtual void Initialize()
        {
            var webHostBuilder = new WebHostBuilder()
             .ConfigureAppConfiguration(config =>
             {
                 config.SetBasePath(Directory.GetCurrentDirectory());
                 config.AddEnvironmentVariables();
                 config.AddJsonFileEx(Path.Combine("RuleEngine", "Config", "rule1.json"));
             })
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
