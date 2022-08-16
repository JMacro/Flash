using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Flash.Extensions.Job.Quartz.QuartzHostingStartup))]
namespace Flash.Extensions.Job.Quartz
{
    internal class QuartzHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddQuartz(null);
            });
        }
    }
}
