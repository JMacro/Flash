using System;

namespace Flash.Extensions.Cache
{
    public class DistributedLockRenewalConfig
    {
        public DistributedLockRenewalConfig(string lockName, string lockValue, TimeSpan lockOutTime, int retryCount = 3)
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
        public string LockName { get; set; }
        /// <summary>
        /// 锁的值
        /// </summary>
        public string LockValue { get; set; }
        /// <summary>
        /// 锁过期时间
        /// </summary>
        public TimeSpan LockOutTime { get; set; }
        /// <summary>
        ///  重试次数
        /// </summary>
        public int RetryCount { get; set; }
        /// <summary>
        /// 当前已重试次数
        /// </summary>
        public int CurrentRetryNumber { get; set; }
        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime AddTime { get; set; }
    }
}
