using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using System.ComponentModel;

namespace Flash.AspNetCore.WorkFlow.Application.Dtos.FlowConfigs
{
    public class FlowConfigResponseDto
    {
        public long Id { get; set; }
        /// <summary>
        /// 父Id
        /// </summary>
        [Description("父Id")]
        public long ParentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }
        /// <summary>
        /// 流程配置类型
        /// </summary>
        [Description("流程配置类型")]
        public EWorkFlowConfigType Type { get; set; }
        /// <summary>
        /// 流程配置类型
        /// </summary>
        [Description("流程配置子类型")]
        public EWorkFlowConfigSubType SubType { get; set; }
        /// <summary>
        /// 分类类型
        /// </summary>
        [Description("分类类型")]
        public short ClassType { get; set; }
        /// <summary>
        /// 分类子类型
        /// </summary>
        [Description("分类子类型")]
        public short ClassSubType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; } = "";
        /// <summary>
        /// 是否删除
        /// </summary>
        [Description("是否删除")]
        public bool IsDelete { get; set; }
    }
}
