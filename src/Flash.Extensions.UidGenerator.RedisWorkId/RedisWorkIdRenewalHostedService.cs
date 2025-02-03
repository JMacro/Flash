using Flash.Extensions.Cache;
using Flash.Extensions.UidGenerator.WorkIdCreateStrategy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.UidGenerator.RedisWorkId
{
    public sealed class RedisWorkIdRenewalHostedService : Microsoft.Extensions.Hosting.IHostedService
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ICacheManager _cacheManager;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IWorkIdCreateStrategy _workIdCreateStrategy;
        private readonly ILogger<RedisWorkIdRenewalHostedService> _logger;
        private readonly string _appId;
        private readonly TimeSpan _cacheRenewTTL;
        private readonly System.Timers.Timer _timer;
        private static object _async = new object();
        private bool _isRun = false;

        public RedisWorkIdRenewalHostedService(
            ICacheManager cacheManager,
            IHostApplicationLifetime lifetime,
            IWorkIdCreateStrategy workIdCreateStrategy,
            ILogger<RedisWorkIdRenewalHostedService> logger,
            string appId,
            TimeSpan cacheRenewTTL,
            int interval = 15)
        {
            this._cancellationTokenSource = new CancellationTokenSource();
            this._cacheManager = cacheManager;
            this._lifetime = lifetime;
            this._workIdCreateStrategy = workIdCreateStrategy;
            this._logger = logger;
            this._appId = appId;
            this._cacheRenewTTL = cacheRenewTTL;
            this._timer = new System.Timers.Timer(interval * 1000);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._lifetime.ApplicationStarted.Register(delegate
            {
                this._logger.LogInformation("Begin work id renewal service ...");
                this._timer.Elapsed += delegate
                {
                    BeginScheduler(this._cancellationTokenSource.Token);
                };
                this._timer.Start();
            });
            this._lifetime.ApplicationStopping.Register(delegate
            {
                this._timer.Stop();
                this._logger.LogInformation("Stop work id renewal service ...");
            });
            return Task.CompletedTask;
        }

        private void BeginScheduler(CancellationToken cancellationToken)
        {
            lock (_async)
            {
                if (this._isRun) return;

                this._isRun = true;

                var workId = this._workIdCreateStrategy.GetWorkId();
                var centerId = this._workIdCreateStrategy.GetCenterId();

                var redisKey = $"{centerId}:workid:{this._appId}:{workId}";
                var keyTime = this._cacheManager.KeyTimeToLive(redisKey);

                if (keyTime.HasValue && keyTime.Value.TotalSeconds <= 150)
                {
                    this._cacheManager.ExpireEntryAt(redisKey, keyTime.Value.Add(_cacheRenewTTL));
                }
                this._isRun = false;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
