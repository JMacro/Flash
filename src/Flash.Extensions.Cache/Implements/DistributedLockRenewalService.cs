using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Cache
{
    /// <summary>
    /// 分布式锁续期服务
    /// </summary>
    public class DistributedLockRenewalService : IDistributedLockRenewalService
    {
        private readonly DistributedLockRenewalCollection _distributedLockRenewalCollection;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DistributedLockRenewalService> _logger;

        public DistributedLockRenewalService(
            DistributedLockRenewalCollection distributedLockRenewalCollection,
            IServiceProvider serviceProvider,
            ILogger<DistributedLockRenewalService> logger)
        {
            this._distributedLockRenewalCollection = distributedLockRenewalCollection;
            this._serviceProvider = serviceProvider;
            this._logger = logger;
        }

        public async void RunRenewal(DistributedLockRenewalCheck check, CancellationToken cancellationToken = default)
        {
            this._logger.LogInformation("Begin run cache renewal...");
            var result = await check.RunAsync(this._serviceProvider, cancellationToken).AsTask();
            if (result.CheckResult)
            {
                _distributedLockRenewalCollection.Remove(result.Data.LockName, result.Data.LockValue);
            }
        }
    }
}
