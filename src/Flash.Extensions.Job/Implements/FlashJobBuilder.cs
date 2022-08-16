using Flash.Core;

namespace Flash.Extensions.Job
{
    public class FlashJobBuilder : IFlashJobBuilder
    {
        private readonly IFlashHostBuilder _flashHost;

        public FlashJobBuilder(IFlashHostBuilder flashHost)
        {
            this._flashHost = flashHost;
        }

        public IFlashHostBuilder FlashHost { get { return this._flashHost; } }
    }
}
