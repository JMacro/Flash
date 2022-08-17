using System;
using System.Collections.Generic;

namespace Flash.Extensions.Job
{
    /// <summary>
    /// Job配置
    /// </summary>
    public class JobConfiguration
    {
        /// <summary>
        /// Job总开关
        /// </summary>
        public bool Open { get; set; }
        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool? IsPersistence { get; set; } = false;
        /// <summary>
        /// 持久化类型
        /// </summary>
        public string PersistenceType { get; set; } = "";
        /// <summary>
        /// 持久化链接
        /// </summary>
        public string PersistenceConnection { get; set; } = "";
        /// <summary>
        /// 持久化前缀
        /// </summary>
        public string PersistencePrefix { get; set; } = "";
        /// <summary>
        /// 触发器集合
        /// </summary>
        public List<CronTriggerConfiguration> CronTriggers { get; set; }
    }

    /// <summary>
    /// 触发器
    /// </summary>
    public class CronTriggerConfiguration
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? Enable { get; set; }
        /// <summary>
        /// Job名称
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// Job类型（完全命名空间）
        /// </summary>
        public string JobType { get; set; }
        /// <summary>
        /// Job分组
        /// </summary>
        public string JobGroup { get; set; }
        /// <summary>
        /// 时区
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Local;
        /// <summary>
        /// Job数据
        /// </summary>
        public IDictionary<string, object> JobData { get; set; }
        /// <summary>
        /// Cron表达式
        /// </summary>
        public string Expression { get; set; }
        /// <summary>
        /// 是否为简单Job
        /// </summary>
        public bool IsSimpleSchedule { get; set; }
    }
}
