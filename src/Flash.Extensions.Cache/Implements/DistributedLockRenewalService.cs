using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Cache
{
    /// <summary>
    /// 分布式锁续期服务
    /// </summary>
    public class DistributedLockRenewalService : Microsoft.Extensions.Hosting.IHostedService
    {
        private readonly IDistributedLock _distributedLock;
        private readonly IDistributedLockRenewalScheduler _distributedLockRenewalScheduler;
        private readonly ILogger<DistributedLockRenewalService> _logger;
        private Timer timer;
        private static object _async = new object();
        private bool _isRun = false;

        public DistributedLockRenewalService(IDistributedLock distributedLock, IDistributedLockRenewalScheduler distributedLockRenewalScheduler, ILogger<DistributedLockRenewalService> logger)
        {
            this._distributedLock = distributedLock;
            this._distributedLockRenewalScheduler = distributedLockRenewalScheduler;
            this._logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation("Begin Distributed Lock Renewal Service");
            timer = new Timer(new TimerCallback(BeginScheduler), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private void BeginScheduler(object state)
        {
            lock (_async)
            {
                if (this._isRun) return;

                this._isRun = true;

                var schedulerList = this._distributedLockRenewalScheduler.SchedulerList.ToArray();
                foreach (var item in schedulerList)
                {
                    if (item.Value.CurrentRetryNumber >= item.Value.RetryCount)
                    {
                        this._distributedLockRenewalScheduler.Remove(item.Value.LockName, item.Value.LockValue);
                        continue;
                    }

                    //判断过期时间是否超过三分之一的过期时间
                    if ((DateTime.Now - item.Value.AddTime).TotalMilliseconds > (item.Value.LockOutTime.TotalMilliseconds / 3))
                    {
                        item.Value.AddTime = DateTime.Now;
                        item.Value.CurrentRetryNumber++;
                        //延期过期时间
                        this._distributedLock.LockRenewal(item.Value.LockName, item.Value.LockValue, item.Value.LockOutTime);
                    }
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
