using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: HostingStartup(typeof(Flash.Extensions.Job.Hangfire.AspNetCore.HangfireHostingStartup))]
namespace Flash.Extensions.Job.Hangfire.AspNetCore
{
    internal class HangfireHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var jobConfiguration = serviceProvider.GetService<IGlobalJobConfiguration>();
                services.AddJobConfiguration(jobConfiguration.SectionName);
                services.AddHangfire();
            });

            builder.Configure(app =>
            {
                var jobConfiguration = app.ApplicationServices.GetService<IGlobalJobConfiguration>();
                app.UseHangfire(jobConfiguration);
            });
        }
    }
}