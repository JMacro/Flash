using System;
using Flash.AspNetCore.WorkFlow.Domain.Core;
using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;

namespace Flash.AspNetCore.WorkFlow.Domain.Events
{
	public class FlowConfigUpdateEvent : DomainEventBase	
    {
        /// <summary>
        /// 父Id
        /// </summary>
        public long ParentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 当Type=1并SubType=2时为流程TypeId
        /// </summary>
        public long ObjectId { get; set; }
        /// <summary>
        /// 流程配置类型
        /// </summary>
        public EWorkFlowConfigType Type { get; set; }
        /// <summary>
        /// 流程配置类型
        /// </summary>
        public EWorkFlowConfigSubType SubType { get; set; }
        /// <summary>
        /// 分类类型
        /// </summary>
        public short ClassType { get; set; }
        /// <summary>
        /// 分类子类型
        /// </summary>
        public short ClassSubType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}

