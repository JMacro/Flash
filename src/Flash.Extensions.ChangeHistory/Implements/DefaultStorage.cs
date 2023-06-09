using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extensions.ChangeHistory
{
    public class DefaultStorage : IStorage
    {
        private readonly ILogger<DefaultStorage> _logger;

        public DefaultStorage(ILogger<DefaultStorage> logger)
        {
            this._logger = logger;
        }

        public Task<IBasePageResponse<ChangeHistoryInfo>> GetPageList(PageSearchQuery page)
        {
            throw new NotImplementedException("请引用寄存器组件");
        }

        public Task<bool> Insert(params ChangeHistoryInfo[] changes)
        {
            this._logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(changes));
            return Task.FromResult(true);
        }
    }
}
