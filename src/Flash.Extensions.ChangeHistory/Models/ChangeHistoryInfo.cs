using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.ChangeHistory
{
    /// <summary>
    /// 变更历史记录
    /// </summary>
    public class ChangeHistoryInfo
    {
        /// <summary>
        /// 变更历史记录
        /// </summary>
        public ChangeHistoryInfo()
        {
        }

        /// <summary>
        /// 实体对象类型
        /// </summary>
        public string EntityType { get; set; }
        /// <summary>
        /// 实体对象Id
        /// </summary>
        public object EntityId { get; set; }
        /// <summary>
        /// 变更的属性
        /// </summary>
        public List<ChangeHistoryPropertyInfo> HistoryPropertys { get; set; }
    }

    /// <summary>
    /// 变更属性信息
    /// </summary>
    public class ChangeHistoryPropertyInfo
    {
        /// <summary>
        /// 实体的属性名，标记实体的哪个属性发生修改
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 原属性值
        /// </summary>
        public string OldValue { get; set; }
        /// <summary>
        /// 新属性值
        /// </summary>
        public string NewValue { get; set; }
    }
}
