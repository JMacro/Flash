using System;

namespace Flash.Extensions.Cache
{
    /// <summary>
    /// 分布式锁
    /// </summary>
    public interface IDistributedLock
    {
        /// <summary>
        /// 加锁
        /// </summary>
        /// <param name="LockName">锁名称</param>
        /// <param name="LockValue">锁的值</param>
        /// <param name="LockOutTime">锁保持时间</param>
        /// <param name="retryAttemptMillseconds">获取锁失败c重试间隔</param>
        /// <param name="retryTimes">最大重试次数</param>
        /// <returns></returns>
        bool Enter(string LockName, string LockValue, TimeSpan LockOutTime, int retryAttemptMillseconds = 50, int retryTimes = 5);

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="LockName">锁名称</param>
        /// <param name="LockValue">锁的值</param>
        void Exit(string LockName, string LockValue);

        /// <summary>
        /// 锁续期
        /// </summary>
        /// <param name="LockName">锁名称</param>
        /// <param name="LockValue">锁的值</param>
        /// <param name="renewalTime">续期时间</param>
        /// <returns></returns>
        bool LockRenewal(string LockName, string LockValue, TimeSpan renewalTime);
    }
}
