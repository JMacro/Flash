using Flash.Extensions.EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test.Web
{
    public class TestEventHandler : IProcessMessageHandler<TestEvent>, IMessageAckHandler
    {
        private readonly ILogger<TestEventHandler> _logger;

        public TestEventHandler(ILogger<TestEventHandler> logger)
        {
            this._logger = logger;
        }


        public Task<bool> Handle(TestEvent message, Dictionary<string, object> headers, CancellationToken cancellationToken)
        {
            this._logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            headers.TryGetValue("x-carrier-id", out var carrierId);
            this._logger.LogInformation(System.Text.Encoding.UTF8.GetString(carrierId as byte[]));
            Thread.Sleep(1000);

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
