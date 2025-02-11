using Flash.Extensions.ORM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.PO
{
    /// <summary>
    /// 表单类型持久化模型
    /// </summary>
    [Table("sys_workflow_formtype")]
    public class FormTypePO : BaseEntity<long>, IEntity<long>, IEntity
    {
        public string Name { get; set; }
        public string FieldDatas { get; set; }
    }
}
