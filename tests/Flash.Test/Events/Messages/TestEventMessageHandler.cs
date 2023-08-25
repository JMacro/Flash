using Flash.Extensions.EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test.Events.Messages
{
    public class TestEventMessageHandler : IProcessMessageHandler<TestEventMessage>, IMessageAckHandler
    {
        private readonly ILogger<TestEventMessageHandler> _logger;

        public TestEventMessageHandler(ILogger<TestEventMessageHandler> logger)
        {
            this._logger = logger;
        }


        public Task<bool> Handle(TestEventMessage message, Dictionary<string, object> headers, CancellationToken cancellationToken)
        {
            this._logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            if (message.Number > 7)
            {
                throw new Exception("模拟异常抛出");
            }
            return Task.FromResult(true);
        }

        public Task AckHandle(MessageResponse message)
        {
            _logger.LogDebug($"ACK: queue {message.QueueName} route={message.RouteKey} messageId:{message.MessageId}");
            return Task.FromResult(true);
        }

        public Task NAckHandle(MessageResponse message, Exception ex)
        {
            _logger.LogDebug($"NAck: queue {message.QueueName} route={message.RouteKey} messageId:{message.MessageId}");
            return Task.FromResult(true);
        }
    }
}
