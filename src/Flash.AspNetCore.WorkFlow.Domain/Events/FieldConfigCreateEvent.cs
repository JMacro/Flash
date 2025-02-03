using System.ComponentModel;
using Flash.AspNetCore.WorkFlow.Domain.Core;
using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;

namespace Flash.AspNetCore.WorkFlow.Domain.Events
{
    public class FieldConfigCreateEvent : DomainEventBase
    {
        /// <summary>
        /// 工作流模块与场景配置Idßß
        /// </summary>
        public long WorkFlowModuleSceneConfigId { get; set; }
        /// <summary>
        /// 字段名
        /// </summary>
        [Description("字段名")]
        public string Name { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [Description("表名")]
        public string TableName { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        [Description("数据类型")]
        public EWorkFlowFieldDataType Type { get; set; }
        /// <summary>
        /// 字段显示名
        /// </summary>
        [Description("字段显示名")]
        public string DisplayName { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [Description("单位")]
        public string Unit { get; set; }
        /// <summary>
        /// 是否单选
        /// </summary>
        [Description("是否单选")]
        public bool IsSingleSelect { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        [Description("是否启用")]
        public bool Enable { get; set; } = true;
        /// <summary>
        /// 执行方法
        /// </summary>
        [Description("执行方法")]
        public string ExecuteMethod { get; set; }
        /// <summary>
        /// 返回结果类型
        /// </summary>
        [Description("返回结果类型")]
        public string ResultType { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int Sort { get; set; }
    }
}
