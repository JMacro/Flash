using Flash.Extersions.Cache;
using Flash.Extersions.DistributedLock;
using Polly;
using System;

namespace Flash.Extersions.DistributedLock
{
    /// <summary>
    /// 分布式锁
    /// </summary>
    public class RedisDistributedLock : IDistributedLock
    {
        private readonly ICacheManager _cacheManager;
        public RedisDistributedLock(ICacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
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
                var cacheKey = "Lock:" + LockName;
                //自旋锁
                do
                {
                    if (!_cacheManager.LockTake(cacheKey, LockValue, LockOutTime))
                    {
                        retryTimes--;
                        if (retryTimes < 0)
                        {
                            return false;
                        }

                        if (retryAttemptMillseconds > 0)
                        {
                            Console.WriteLine($"Wait Lock {LockName} to {retryAttemptMillseconds} millseconds");
                            //获取锁失败则进行锁等待
                            System.Threading.Thread.Sleep(retryAttemptMillseconds);
                        }
                    }
                    else
                    {
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
                        Console.WriteLine($"执行异常,重试次数：{retryCount},【异常来自：{exception.GetType().Name}】.");
                    });

                polly.Execute(() =>
                {
                    _cacheManager.LockRelease("Lock:" + LockName, LockValue);
                });
            }
        }
    }
}
