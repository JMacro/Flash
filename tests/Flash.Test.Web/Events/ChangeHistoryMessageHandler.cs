using Flash.Extensions.ChangeHistory;
using Flash.Extensions.EventBus;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test.Web
{
    public class ChangeHistoryMessageHandler : IProcessMessageHandler<ChangeHistoryInfo>, IMessageAckHandler
    {
        public Task AckHandle(MessageResponse response)
        {
            return Task.FromResult(true);
        }

        public Task<bool> Handle(ChangeHistoryInfo message, Dictionary<string, object> headers, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task NAckHandle(MessageResponse response, Exception ex)
        {
            return Task.FromResult(true);
        }
    }
}
