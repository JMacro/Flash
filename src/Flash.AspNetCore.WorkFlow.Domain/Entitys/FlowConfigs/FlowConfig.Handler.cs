using Flash.AspNetCore.WorkFlow.Domain.Core;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FieldConfigs;
using Flash.AspNetCore.WorkFlow.Domain.Events;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs
{
    public partial class FlowConfig : IAggregateRootHandler<FieldConfigCreateEvent>
    {
        public void Handle(FlowConfigCreateEvent @event)
        {
            this.Id = @event.Id;
            this.ParentId = @event.ParentId;
            this.Name = @event.Name;
            this.ObjectId = @event.ObjectId;
            this.Type = @event.Type;
            this.SubType = @event.SubType;
            this.ClassType = @event.ClassType;
            this.ClassSubType = @event.ClassSubType;
            this.Remark = @event.Remark;
        }

        public void Handle(FlowConfigUpdateEvent @event)
        {
            this.Id = @event.Id;
            this.ParentId = @event.ParentId;
            this.Name = @event.Name;
            this.ObjectId = @event.ObjectId;
            this.Type = @event.Type;
            this.SubType = @event.SubType;
            this.ClassType = @event.ClassType;
            this.ClassSubType = @event.ClassSubType;
            this.Remark = @event.Remark;
        }

        public void Handle(FieldConfigCreateEvent @event)
        {
            _fieldConfigs.Add(FieldConfig.Create(new FieldConfigSaveData
            {
                Id = @event.Id,
                WorkFlowModuleSceneConfigId = @event.WorkFlowModuleSceneConfigId,
                Name = @event.Name,
                TableName = @event.TableName,
                Type = @event.Type,
                DisplayName = @event.DisplayName,
                Unit = @event.Unit,
                IsSingleSelect = @event.IsSingleSelect,
                Enable = @event.Enable,
                ExecuteMethod = @event.ExecuteMethod,
                ResultType = @event.ResultType,
                Sort = @event.Sort,
            }));
        }

        public void Handle(FieldConfigUpdateEvent @event)
        {
            _fieldConfigs.Add(FieldConfig.Update(new FieldConfigSaveData
            {
                Id = @event.Id,
                WorkFlowModuleSceneConfigId = @event.WorkFlowModuleSceneConfigId,
                Name = @event.Name,
                TableName = @event.TableName,
                Type = @event.Type,
                DisplayName = @event.DisplayName,
                Unit = @event.Unit,
                IsSingleSelect = @event.IsSingleSelect,
                Enable = @event.Enable,
                ExecuteMethod = @event.ExecuteMethod,
                ResultType = @event.ResultType,
                Sort = @event.Sort,
            }));
        }
    }
}
