using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Flash.Extensions.Cache
{
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

        public bool Add(string lockName, string lockValue, TimeSpan lockOutTime)
        {
            this._logger.LogInformation($"Add distributed lock renewal scheduler[{lockName}-{lockValue}]");
            return _schedulerList.TryAdd($"{lockName}-{lockValue}", new DistributedLockRenewalCheck(lockName, lockValue, lockOutTime));
        }

        public bool Remove(string lockName, string lockValue)
        {
            this._logger.LogInformation($"Remove distributed lock renewal scheduler[{lockName}-{lockValue}]");
            return _schedulerList.TryRemove($"{lockName}-{lockValue}", out var value);
        }

        public IReadOnlyList<DistributedLockRenewalCheck> ToList()
        {
            return _schedulerList.Values.Where(p => !p.IsRunCheck).ToList().AsReadOnly();
        }
    }
}
