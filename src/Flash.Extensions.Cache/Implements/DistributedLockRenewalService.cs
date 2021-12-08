using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Cache
{
    public class DistributedLockRenewalService : IDistributedLockRenewalService
    {
        private readonly DistributedLockRenewalCollection _distributedLockRenewalCollection;
        private readonly IServiceProvider _serviceProvider;

        public DistributedLockRenewalService(
            DistributedLockRenewalCollection distributedLockRenewalCollection,
            IServiceProvider serviceProvider)
        {
            this._distributedLockRenewalCollection = distributedLockRenewalCollection;
            this._serviceProvider = serviceProvider;
        }

        public async void RunRenewal(DistributedLockRenewalCheck check, CancellationToken cancellationToken = default)
        {
            var result = await check.RunAsync(this._serviceProvider, cancellationToken).AsTask();
            if (result.CheckResult)
            {
                _distributedLockRenewalCollection.Remove(result.Data.LockName, result.Data.LockValue);
            }
        }
    }
}
