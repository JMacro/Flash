using Flash.Extensions.EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test.Web
{
    public class TestDelayHandler : IProcessMessageHandler<TestDelayMessage>, IMessageAckHandler
    {
        private readonly ILogger<TestDelayHandler> _logger;

        public TestDelayHandler(ILogger<TestDelayHandler> logger)
        {
            this._logger = logger;
        }


        public Task<bool> Handle(TestDelayMessage message, Dictionary<string, object> headers, CancellationToken cancellationToken)
        {
            this._logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            Random r = new Random();
            var va = r.NextDouble() * 9 + 1;
            if (va > 5)
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
