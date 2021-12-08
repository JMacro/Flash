using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Extensions.Cache
{
    public sealed class DistributedLockRenewalCheck
    {
        private volatile int _writerCount;

        public DistributedLockRenewalCheck(string lockName, string lockValue, TimeSpan lockOutTime, int retryCount = 3)
        {
            this.LockName = lockName;
            this.LockValue = lockValue;
            this.LockOutTime = lockOutTime;
            this.RetryCount = retryCount;
            this.CurrentRetryNumber = 0;
            this.AddTime = DateTime.Now;
        }

        /// <summary>
        /// 锁名称
        /// </summary>
        public string LockName { get; private set; }
        /// <summary>
        /// 锁的值
        /// </summary>
        public string LockValue { get; private set; }
        /// <summary>
        /// 锁过期时间
        /// </summary>
        public TimeSpan LockOutTime { get; private set; }
        /// <summary>
        ///  重试次数
        /// </summary>
        public int RetryCount { get; private set; }
        /// <summary>
        /// 当前已重试次数
        /// </summary>
        public int CurrentRetryNumber { get; private set; }
        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime AddTime { get; private set; }
        /// <summary>
        /// 是否启动检查
        /// </summary>
        public bool IsRunCheck { get; private set; } = false;

        public async ValueTask<DistributedLockRenewalCheckResult> RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default(CancellationToken))
        {
            var distributedLock = serviceProvider.GetService<IDistributedLock>();
            this.IsRunCheck = true;
            while (this.CurrentRetryNumber < this.RetryCount)
            {
                if (Interlocked.Exchange(ref _writerCount, 1) != 0)
                {
                    await Task.Delay(5, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                //判断过期时间是否超过三分之一的过期时间
                if ((DateTime.Now - this.AddTime).TotalMilliseconds > (this.LockOutTime.TotalMilliseconds / 3))
                {
                    this.AddTime = DateTime.Now;
                    this.CurrentRetryNumber++;
                    //延期过期时间
                    distributedLock.LockRenewal(this.LockName, this.LockValue, this.LockOutTime);
                }

                _writerCount = 0;
            }

            return DistributedLockRenewalCheckResult.Create(this, true);
        }
    }
}
