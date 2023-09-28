using Flash.AspNetCore.WorkFlow.Domain.Entitys;
using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using Flash.AspNetCore.WorkFlow.Infrastructure.PO;
using Flash.Extensions;
using System.ComponentModel;

namespace Flash.AspNetCore.WorkFlow.Application.Dtos.FlowFieldConfigs
{
    [AutoMapperTo(typeof(FlowFieldConfigPO))]
    public class FlowFieldConfigResponseDto
    {
        /// <summary>
        /// 工作流字段Id
        /// </summary>
        [Description("工作流字段Id")]
        public long Id { get; set; }
        /// <summary>
        /// 工作流模块与场景配置Id
        /// </summary>
        [Description("工作流模块与场景配置Id")]
        public long WorkFlowModuleSceneConfigId { get; set; }
        /// <summary>
        /// 字段名
        /// </summary>
        [Description("字段名")]
        public string Name { get; set; }
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
        public bool Enable { get; set; }
        /// <summary>
        /// 字段值集合
        /// </summary>
        public object Values { get; set; }
    }
}
