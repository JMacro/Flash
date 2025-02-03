using Flash.AspNetCore.WorkFlow.Domain.Entitys.FormTypes;
using MediatR;
using System.Collections.Generic;

namespace Flash.AspNetCore.WorkFlow.Domain.Commands.FlowForms.DefineFlowForm
{
    /// <summary>
    /// 定义表单类型命令
    /// </summary>
    public class DefineFlowFormCommand : IRequest<FormType>
    {
        public string Name { get; set; }
        public IDictionary<string, string> Fields { get; set; }
    }
}
