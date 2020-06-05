using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;


namespace Flash.Test.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseIISIntegration()
            .UseKestrel()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFileEx("Config/appsettings.json", false, true)
                .AddJsonFileEx("Config/metrics.json", false, true)
                .AddJsonFileEx("Config/redis.json", false, true)
                .AddEnvironmentVariables()
                .AddCommandLine(args).Build();
            })
            .UseMetrics("AppMetrics")
            .UseStartup<Startup>();
    }
}
