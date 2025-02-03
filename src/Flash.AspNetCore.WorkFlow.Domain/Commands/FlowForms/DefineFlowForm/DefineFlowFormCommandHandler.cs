using Flash.AspNetCore.WorkFlow.Domain.Entitys.FormTypes;
using Flash.AspNetCore.WorkFlow.Infrastructure.DomainEventsDispatching;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.AspNetCore.WorkFlow.Domain.Commands.FlowForms.DefineFlowForm
{
    /// <summary>
    /// 定义表单类型命令处理器
    /// </summary>
    internal class DefineFlowFormCommandHandler : IRequestHandler<DefineFlowFormCommand, FormType>
    {
        private readonly IDomainEventsDispatcher _domainEventsDispatcher;

        public DefineFlowFormCommandHandler(IDomainEventsDispatcher domainEventsDispatcher)
        {
            this._domainEventsDispatcher = domainEventsDispatcher;
        }

        public Task<FormType> Handle(DefineFlowFormCommand request, CancellationToken cancellationToken)
        {
            var formType = new FormType();
            formType.Init(request.Name, request.Fields);

            return Task.FromResult(formType);
        }
    }
}
