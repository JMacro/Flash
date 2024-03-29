﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Flash.Extensions.Cache
{
    /// <summary>
    /// 分布式锁自动续期调度器
    /// </summary>
    public interface IDistributedLockRenewalScheduler
    {
        /// <summary>
        /// 添加到自动续期调度器
        /// </summary>
        /// <param name="lockName">锁名称</param>
        /// <param name="lockValue">锁的值</param>
        /// <param name="lockOutTime">锁保持时间</param>
        /// <returns></returns>
        bool Add(string lockName, string lockValue, TimeSpan lockOutTime);
        /// <summary>
        /// 移除自动续期调度器
        /// </summary>
        /// <param name="lockName">锁名称</param>
        /// <param name="lockValue">锁的值</param>
        /// <returns></returns>
        bool Remove(string lockName, string lockValue);
        /// <summary>
        /// 获得调度列表
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<DistributedLockRenewalCheck> GetSchedulerList();
    }
}
