using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Repositories;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FieldConfigs;
using Flash.AspNetCore.WorkFlow.Domain.Events;
using Flash.Extensions;
using MediatR;

namespace Flash.AspNetCore.WorkFlow.Domain.Notifications
{
    public class FieldConfigUpdateEventNotification : INotificationHandler<FieldConfigUpdateEvent>
    {
        private readonly IFlowFieldConfigRepository _flowFieldConfigRepository;

        public FieldConfigUpdateEventNotification(
            IFlowFieldConfigRepository flowFieldConfigRepository)
        {
            this._flowFieldConfigRepository = flowFieldConfigRepository;
        }

        public Task Handle(FieldConfigUpdateEvent notification, CancellationToken cancellationToken)
        {
            var fieldConfigs = this._flowFieldConfigRepository.Table.FirstOrDefault(p => p.Name == notification.Name && p.WorkFlowModuleSceneConfigId == notification.WorkFlowModuleSceneConfigId);
            fieldConfigs.ThrowIfNull("字段名称不存在，无法进行操作");

            var fieldConfig = FieldConfig.New();
            notification.Id = fieldConfigs.Id;
            fieldConfig.Load(notification);
            fieldConfig.CheckExecuteMethodImplemented();
            this._flowFieldConfigRepository.Update(fieldConfig);

            return Task.CompletedTask;
        }
    }
}

