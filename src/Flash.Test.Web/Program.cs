using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Flash.Extensions.HealthChecks;

namespace Flash.Test.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Environment.SetEnvironmentVariable("Environment", env ?? "");

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFileEx("Config/appsettings.json", false, true)
                    .AddJsonFileEx("Config/metrics.json", false, true)
                    .AddJsonFileEx("Config/redis.json", false, true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args).Build();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddLog4Net("Config/log4net.xml", true);
                    logging.AddDebug();
                    logging.AddConsole();
                })
                .UseMetrics("AppMetrics")
                .UseHealthChecks("/healthcheck")
                .ConfigureFlash(setup =>
                {
                    //setup.UseHangfireHostingStartup();
                    //setup.UseQuartzHostingStartup();
                })
                .UseStartup<Startup>();
    }
}
