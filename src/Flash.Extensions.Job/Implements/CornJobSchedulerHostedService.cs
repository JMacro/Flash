using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Job
{
    public class CornJobSchedulerHostedService : Microsoft.Extensions.Hosting.IHostedService
    {
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ICornJobScheduler _cornJobScheduler;
        private readonly int _managedThreadId = -1;

        public CornJobSchedulerHostedService(
            ICornJobScheduler cornJobScheduler,
            ILogger<CornJobSchedulerHostedService> logger)
        {
            this._logger = logger;
            this._cornJobScheduler = cornJobScheduler;
            this._cancellationTokenSource = new CancellationTokenSource();
            this._managedThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await this._cornJobScheduler.RunAsync(cancellationToken);
                _logger.LogInformation($"Create thread id:{this._managedThreadId} runner cornjob scheduler started");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await this._cornJobScheduler.ShutdownAsync(cancellationToken);
            _logger.LogInformation($"thread id:{this._managedThreadId} runner cornjob scheduler stoped");
            _cancellationTokenSource.Cancel();
        }
    }
}
