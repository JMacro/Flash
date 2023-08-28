using Flash.Extensions.ChangeHistory;
using Flash.Extensions.EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test.EntityChange.Events
{
    public class ChangeHistoryMessageHandler : IProcessMessageHandler<ChangeHistoryInfo>, IMessageAckHandler
    {
        private readonly ILogger<ChangeHistoryMessageHandler> _logger;

        public ChangeHistoryMessageHandler(ILogger<ChangeHistoryMessageHandler> logger)
        {
            this._logger = logger;
        }

        public Task<bool> Handle(ChangeHistoryInfo message, Dictionary<string, object> headers, CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"接收到MQ消息：{Newtonsoft.Json.JsonConvert.SerializeObject(message)}");
            return Task.FromResult(true);
        }

        public Task AckHandle(MessageResponse response)
        {
            return Task.FromResult(true);
        }

        

        public Task NAckHandle(MessageResponse response, Exception ex)
        {
            return Task.FromResult(true);
        }
    }
}
