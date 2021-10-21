using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Flash.Extensions.Cache
{
    /// <summary>
    /// 分布式锁自动续期调度器
    /// </summary>
    public class DistributedLockRenewalScheduler : IDistributedLockRenewalScheduler
    {
        private readonly ILogger<DistributedLockRenewalScheduler> _logger;

        public ConcurrentDictionary<string, DistributedLockRenewalConfig> SchedulerList { get; private set; } = new ConcurrentDictionary<string, DistributedLockRenewalConfig>();

        public DistributedLockRenewalScheduler(ILogger<DistributedLockRenewalScheduler> logger)
        {
            this._logger = logger;
        }

        public bool Add(string lockName, string lockValue, TimeSpan lockOutTime)
        {
            this._logger.LogInformation($"Add distributed lock renewal scheduler[{lockName}-{lockValue}]");
            return SchedulerList.TryAdd($"{lockName}-{lockValue}", new DistributedLockRenewalConfig(lockName, lockValue, lockOutTime));
        }

        public bool Remove(string lockName, string lockValue)
        {
            this._logger.LogInformation($"Remove distributed lock renewal scheduler[{lockName}-{lockValue}]");
            return SchedulerList.TryRemove($"{lockName}-{lockValue}", out var value);
        }
    }
}
