using Flash.Extensions.EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test.Events.Messages
{
    public class TestEvent2MessageHandler : IProcessMessageHandler<TestEvent2Message>
    {
        private readonly ILogger<TestEvent2MessageHandler> _logger;

        public TestEvent2MessageHandler(ILogger<TestEvent2MessageHandler> logger)
        {
            this._logger = logger;
        }

        public Task<bool> Handle(TestEvent2Message message, Dictionary<string, object> headers, CancellationToken cancellationToken)
        {
            this._logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            if (message.Number > 7)
            {
                throw new Exception("模拟异常抛出");
            }
            return Task.FromResult(true);
        }
    }
}
