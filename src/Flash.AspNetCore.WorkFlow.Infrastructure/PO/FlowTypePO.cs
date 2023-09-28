using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Text;
using Flash.Extensions.ORM;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.PO
{
    /// <summary>
    /// 工作流类型实体
    /// </summary>
    [Table("sys_workflow_flowtypes")]
    [Description("工作流类型实体")]
    public class FlowTypePO : BaseEntity<long>, IEntity<long>, IEntity
    {
        public string Name { get; set; }
        public string SettingData { get; set; }
        public string NodeDatas { get; set; }
        public string AttachFormDatas { get; set; }
        public string FlowEvents { get; set; }
        public string FormTypeId { get; set; }
        public bool Completed { get; set; }
        public bool IsDisable { get; set; }
        public bool IsDraft { get; set; }
        public string Description { get; set; }
    }
}
