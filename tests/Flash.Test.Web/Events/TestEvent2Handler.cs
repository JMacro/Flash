using Flash.Extensions.EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test.Web
{
    public class TestEvent2Handler : IProcessMessageHandler<TestEvent2>
    {
        private readonly ILogger<TestEvent2Handler> _logger;

        public TestEvent2Handler(ILogger<TestEvent2Handler> logger)
        {
            this._logger = logger;
        }

        public Task<bool> Handle(TestEvent2 message, Dictionary<string, object> headers, CancellationToken cancellationToken)
        {
            this._logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            Thread.Sleep(5000);
            return Task.FromResult(true);
        }
    }
}
