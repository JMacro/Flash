using Microsoft.AspNetCore.Hosting;

namespace Flash.Core
{
    public class FlashWebHostBuilder : IFlashWebHostBuilder
    {
        private readonly IWebHostBuilder _builder;


        public FlashWebHostBuilder(IWebHostBuilder builder)
        {
            this._builder = builder;
        }
        public IWebHostBuilder HostBuilder => _builder;
    }
}

