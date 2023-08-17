using Microsoft.AspNetCore.Hosting;

namespace Flash.Core
{
    public interface IFlashWebHostBuilder
    {
        IWebHostBuilder HostBuilder { get; }
    }
}

