using System;

namespace Flash.Extensions.ORM
{
    /// <summary>
    /// 实体变更跟踪
    /// </summary>
    public sealed class EntityChangeTracker
    {
        /// <summary>
        /// 实体状态
        /// </summary>
        public EntityState State { get; set; }
        /// <summary>
        /// 上下文类型
        /// </summary>
        public Type ContextType { get; set; }
        /// <summary>
        /// 新值
        /// </summary>
        public Object CurrentEntity { get; set; }
        /// <summary>
        /// 原始值
        /// </summary>
        public Object OriginalEntity { get; set; }
    }
}
