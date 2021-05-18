using Flash.Extensions.EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test.Web
{
    public class TestEventHandler : IProcessMessageHandler<TestEvent>
    {
        private readonly ILogger<TestEventHandler> _logger;

        public TestEventHandler(ILogger<TestEventHandler> logger)
        {
            this._logger = logger;
        }

        public Task<bool> Handle(TestEvent message, Dictionary<string, object> headers, CancellationToken cancellationToken)
        {
            this._logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            Thread.Sleep(5000);
            return Task.FromResult(true);
        }
    }
}
