using Microsoft.Extensions.Hosting;
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
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IApplicationLifetime _lifetime;
        private readonly DistributedLockRenewalCollection _distributedLockRenewalCollection;
        private readonly IDistributedLockRenewalService _distributedLockRenewalService;
        private readonly ILogger<DistributedLockRenewalHostedService> _logger;
        private readonly System.Timers.Timer _timer;
        private static object _async = new object();
        private bool _isRun = false;

        public DistributedLockRenewalHostedService(
            IApplicationLifetime lifetime,
            DistributedLockRenewalCollection distributedLockRenewalCollection,
            IDistributedLockRenewalService distributedLockRenewalService,
            ILogger<DistributedLockRenewalHostedService> logger)
        {
            this._cancellationTokenSource = new CancellationTokenSource();
            this._lifetime = lifetime;
            this._distributedLockRenewalCollection = distributedLockRenewalCollection;
            this._distributedLockRenewalService = distributedLockRenewalService;
            this._logger = logger;
            this._timer = new System.Timers.Timer(5);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._lifetime.ApplicationStarted.Register(delegate
            {
                this._logger.LogInformation("Begin Distributed Lock Renewal Service");
                this._timer.Elapsed += delegate
                {
                    BeginScheduler(_cancellationTokenSource.Token);
                };
                this._timer.Start();
            });

            this._lifetime.ApplicationStopping.Register(delegate
            {
                this._timer.Stop();
                this._logger.LogInformation("Stopped Distributed Lock Renewal Service");
            });
            return Task.CompletedTask;
        }

        private void BeginScheduler(CancellationToken cancellationToken)
        {
            lock (_async)
            {
                if (this._isRun) return;

                this._isRun = true;

                var schedulerList = this._distributedLockRenewalCollection.ToList();
                foreach (var item in schedulerList)
                {
                    Task.Factory.StartNew(() =>
                    {
                        this._distributedLockRenewalService.RunRenewal(item, cancellationToken);
                    }, cancellationToken);
                }
                Thread.Sleep(10);
                this._isRun = false;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._cancellationTokenSource.Cancel();
            if (_timer != null)
            {
                _timer.Dispose();
            }
            return Task.CompletedTask;
        }
    }
}
