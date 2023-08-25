using Flash.Extensions.EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extensions.ChangeHistory
{
    public class RabbitMQStorage : IStorage
    {
        private readonly ILogger<RabbitMQStorage> _logger;
        private readonly IEventBus _eventBus;

        public RabbitMQStorage(ILogger<RabbitMQStorage> logger, IEventBus eventBus)
        {
            this._logger = logger;
            this._eventBus = eventBus;
        }

        public Task<IBasePageResponse<ChangeHistoryInfo>> GetPageList(PageSearchQuery page)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Insert(params ChangeHistoryInfo[] changes)
        {
            return await this._eventBus.PublishAsync(changes.Select(p => MessageCarrier.Fill(p)).ToList());
        }
    }
}
