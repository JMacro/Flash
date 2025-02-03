using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Flash.Extensions.Cache
{
    /// <summary>
    /// 分布式锁续期集合
    /// </summary>
    public sealed class DistributedLockRenewalCollection
    {
        private readonly ConcurrentDictionary<string, DistributedLockRenewalCheck> _schedulerList;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DistributedLockRenewalCollection> _logger;

        public DistributedLockRenewalCollection(IServiceProvider serviceProvider)
        {
            this._schedulerList = new ConcurrentDictionary<string, DistributedLockRenewalCheck>();
            this._serviceProvider = serviceProvider;
            this._logger = this._serviceProvider.GetService<ILogger<DistributedLockRenewalCollection>>();
        }

        /// <summary>
        /// 添加到分布式锁续期集合
        /// </summary>
        /// <param name="lockName"></param>
        /// <param name="lockValue"></param>
        /// <param name="lockOutTime"></param>
        /// <returns></returns>
        public bool Add(string lockName, string lockValue, TimeSpan lockOutTime)
        {
            this._logger.LogInformation($"Add distributed lock renewal scheduler[{lockName}-{lockValue}]");
            return _schedulerList.TryAdd($"{lockName}-{lockValue}", new DistributedLockRenewalCheck(lockName, lockValue, lockOutTime));
        }

        /// <summary>
        /// 从分布式锁续期集合移除
        /// </summary>
        /// <param name="lockName"></param>
        /// <param name="lockValue"></param>
        /// <returns></returns>
        public bool Remove(string lockName, string lockValue)
        {
            this._logger.LogInformation($"Remove distributed lock renewal scheduler[{lockName}-{lockValue}]");
            return _schedulerList.TryRemove($"{lockName}-{lockValue}", out var value);
        }

        /// <summary>
        /// 获得分布式锁续期集合
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<DistributedLockRenewalCheck> ToList()
        {
            return _schedulerList.Values.Where(p => !p.IsRunCheck).ToList().AsReadOnly();
        }
    }
}
