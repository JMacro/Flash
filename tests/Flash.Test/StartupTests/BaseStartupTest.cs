using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Test.StartupTests
{
    public abstract class BaseStartupTest
    {
        public IConfiguration Configuration { get; }

        public BaseStartupTest(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {

        }

        public virtual void Configure(IApplicationBuilder app)
        {

        }
    }
}
