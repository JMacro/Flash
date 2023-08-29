using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Cache
{
    /// <summary>
    /// 分布式锁续期Host服务
    /// </summary>
    public class DistributedLockRenewalHostedService : Microsoft.Extensions.Hosting.IHostedService
    {
        private readonly DistributedLockRenewalCollection _distributedLockRenewalCollection;
        private readonly IDistributedLockRenewalService _distributedLockRenewalService;
        private readonly ILogger<DistributedLockRenewalHostedService> _logger;
        private Timer timer;
        private static object _async = new object();
        private bool _isRun = false;

        public DistributedLockRenewalHostedService(
            DistributedLockRenewalCollection distributedLockRenewalCollection,
            IDistributedLockRenewalService distributedLockRenewalService,
            ILogger<DistributedLockRenewalHostedService> logger)
        {
            this._distributedLockRenewalCollection = distributedLockRenewalCollection;
            this._distributedLockRenewalService = distributedLockRenewalService;
            this._logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation("Begin Distributed Lock Renewal Service");
            timer = new Timer(new TimerCallback(BeginScheduler), cancellationToken, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private void BeginScheduler(object state)
        {
            lock (_async)
            {
                if (this._isRun) return;

                this._isRun = true;

                var schedulerList = this._distributedLockRenewalCollection.ToList();
                var ct = (CancellationToken)state;
                foreach (var item in schedulerList)
                {
                    Task.Factory.StartNew(() =>
                    {
                        this._distributedLockRenewalService.RunRenewal(item, ct);
                    }, ct);
                }
                Thread.Sleep(10);
                this._isRun = false;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
            this._logger.LogInformation("Stopped Distributed Lock Renewal Service");
            return Task.CompletedTask;
        }
    }
}
