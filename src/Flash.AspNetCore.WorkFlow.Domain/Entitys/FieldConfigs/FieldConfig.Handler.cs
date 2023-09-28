using Flash.AspNetCore.WorkFlow.Domain.Core;
using Flash.AspNetCore.WorkFlow.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.FieldConfigs
{
    public partial class FieldConfig : IAggregateRootHandler<FieldConfigSaveEvent>
    {
        public void Handle(FieldConfigSaveEvent @event)
        {
            Id = @event.Id;
            WorkFlowModuleSceneConfigId = @event.WorkFlowModuleSceneConfigId;
            Name = @event.Name;
            TableName = @event.TableName;
            Type = @event.Type;
            DisplayName = @event.DisplayName;
            Unit = @event.Unit;
            IsSingleSelect = @event.IsSingleSelect;
            Enable = @event.Enable;
            ExecuteMethod = @event.ExecuteMethod;
            ResultType = @event.ResultType;
            Sort = @event.Sort;
        }
    }
}
