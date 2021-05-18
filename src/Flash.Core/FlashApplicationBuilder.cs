
using Microsoft.AspNetCore.Builder;

namespace Flash.Core
{
    public class FlashApplicationBuilder : IFlashApplicationBuilder
    {
        private readonly IApplicationBuilder _app;

        public FlashApplicationBuilder(IApplicationBuilder app)
        {

            this._app = app;
        }

        public IApplicationBuilder app
        {
            get
            {
                return _app;
            }
        }
    }
}