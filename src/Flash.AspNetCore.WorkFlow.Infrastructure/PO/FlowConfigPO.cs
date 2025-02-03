using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using Flash.Extensions.ORM;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.PO
{
    /// <summary>
    /// 流程配置实体
    /// </summary>
    //[Table("sys_workflow_config")]
    [Description("流程配置实体")]
    public class FlowConfigPO : BaseEntity<long>, IEntity<long>, IEntity
    {
        /// <summary>
        /// 父Id
        /// </summary>
        [Description("父Id")]
        public long ParentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        [StringLength(64)]
        public string Name { get; set; }
        /// <summary>
        /// 当Type=1并SubType=2时为流程TypeId
        /// </summary>
        [Description("对接方目标Id")]
        [StringLength(64)]
        public long ObjectId { get; set; }
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
    }
}
