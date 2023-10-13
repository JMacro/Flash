using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.RuleEngine
{
    internal sealed class RegisterRulesHostedService : Microsoft.Extensions.Hosting.IHostedService
    {
        private readonly IRulesEngineStorage _engineStorage;
        private readonly IRulesEngineClient _rulesEngine;
        private readonly ILogger<RegisterRulesHostedService> _logger;
        private readonly int _managedThreadId = -1;

        public RegisterRulesHostedService(
            IRulesEngineStorage engineStorage, 
            IRulesEngineClient rulesEngine, 
            ILogger<RegisterRulesHostedService> logger)
        {
            this._engineStorage = engineStorage;
            this._rulesEngine = rulesEngine;
            this._logger = logger;
            this._managedThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var configs = await this._engineStorage.GetAll();
                this._rulesEngine.RegisterRules(configs);
                this._logger.LogInformation($"Create thread id:{this._managedThreadId} runner register rules started");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"thread id:{this._managedThreadId} runner register rules stoped");
            return Task.CompletedTask;
        }
    }
}
