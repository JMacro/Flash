using Flash.AspNetCore.WorkFlow.Domain.Core;
using Flash.AspNetCore.WorkFlow.Domain.Events;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.FormTypes
{
    internal partial class FormType : IAggregateRootHandler<FormTypeInitEvent>
    {
        public void Handle(FormTypeInitEvent request)
        {
            Id = request.AggregateId;
            Name = request.Name;
            Fields = request.Fields;
            Version = request.Version;
        }
    }
}
