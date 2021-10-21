using Flash.Extensions.Cache;
using Microsoft.Extensions.Logging;
using Polly;
using System;

namespace Flash.Extensions.Cache.Redis
{
    /// <summary>
    /// 分布式锁
    /// </summary>
    public class DistributedLock : IDistributedLock
    {
        private readonly ICacheManager _cacheManager;
        private readonly IDistributedLockRenewalScheduler _distributedLockRenewalScheduler;
        private readonly ILogger<DistributedLock> _logger;
        private readonly string _KeyPrefix = "Lock";

        public DistributedLock(ICacheManager cacheManager, IDistributedLockRenewalScheduler distributedLockRenewalScheduler, ILogger<DistributedLock> logger)
        {
            this._cacheManager = cacheManager;
            this._distributedLockRenewalScheduler = distributedLockRenewalScheduler;
            this._logger = logger;
        }

        /// <summary>
        /// 加锁
        /// </summary>
        /// <param name="LockName">锁名称</param>
        /// <param name="LockValue">锁的值</param>
        /// <param name="LockOutTime">锁保持时间</param>
        /// <param name="retryAttemptMillseconds">获取锁失败c重试间隔</param>
        /// <param name="retryTimes">最大重试次数</param>
        /// <returns></returns>
        public bool Enter(string LockName, string LockValue, TimeSpan LockOutTime, int retryAttemptMillseconds = 50, int retryTimes = 5)
        {
            if (_cacheManager != null)
            {
                //自旋锁
                do
                {
                    var keyName = AddSysCustomKey(LockName);
                    if (!_cacheManager.LockTake(keyName, LockValue, LockOutTime))
                    {
                        retryTimes--;
                        if (retryTimes < 0)
                        {
                            return false;
                        }

                        if (retryAttemptMillseconds > 0)
                        {
                            this._logger.LogInformation($"Wait Lock {keyName} to {retryAttemptMillseconds} millseconds");
                            //获取锁失败则进行锁等待
                            System.Threading.Thread.Sleep(retryAttemptMillseconds);
                        }
                    }
                    else
                    {
                        this._distributedLockRenewalScheduler.Add(LockName, LockValue, LockOutTime);
                        return true;
                    }
                }
                while (retryTimes > 0);
            }

            //获取锁超时返回
            return false;
        }

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="LockName">锁名称</param>
        /// <param name="LockValue">锁的值</param>
        public void Exit(string LockName, string LockValue)
        {
            if (_cacheManager != null)
            {
                var polly = Policy.Handle<Exception>()
                    .WaitAndRetry(10, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt)), (exception, timespan, retryCount, context) =>
                    {
                        this._logger.LogError($"执行异常,重试次数：{retryCount},【异常来自：{exception.GetType().Name}】.");
                    });

                polly.Execute(() =>
                {
                    var keyName = AddSysCustomKey(LockName);
                    this._distributedLockRenewalScheduler.Remove(keyName, LockValue);
                    _cacheManager.LockRelease(keyName, LockValue);
                });
            }
        }

        /// <summary>
        /// 锁续期
        /// </summary>
        /// <param name="LockName">锁名称</param>
        /// <param name="LockValue">锁的值</param>
        /// <param name="renewalTime">续期时间</param>
        /// <returns></returns>
        public void LockRenewal(string LockName, string LockValue, TimeSpan renewalTime)
        {
            //TODO 待优化：可通过Lua脚本，校验缓存中指定的锁的值是否为要续期锁的值
            if (_cacheManager != null)
            {
                var keyName = AddSysCustomKey(LockName);
                this._logger.LogInformation($"Renewal Lock {keyName} to {renewalTime.TotalMilliseconds} millseconds");
                _cacheManager.ExpireEntryAt(keyName, renewalTime);
            }
        }

        private string AddSysCustomKey(string oldKey)
        {
            return $"{_KeyPrefix}:{oldKey}";
        }
    }
}
