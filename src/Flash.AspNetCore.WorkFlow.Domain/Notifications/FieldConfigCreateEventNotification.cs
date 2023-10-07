using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Repositories;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FieldConfigs;
using Flash.AspNetCore.WorkFlow.Domain.Events;
using Flash.Extensions;
using Flash.Extensions.UidGenerator;
using MediatR;

namespace Flash.AspNetCore.WorkFlow.Domain.Notifications
{
    public class FieldConfigCreateEventNotification : INotificationHandler<FieldConfigCreateEvent>
	{
        private readonly IUniqueIdGenerator _uniqueIdGenerator;
        private readonly IFlowFieldConfigRepository _flowFieldConfigRepository;

        public FieldConfigCreateEventNotification(
            IUniqueIdGenerator uniqueIdGenerator,
            IFlowFieldConfigRepository flowFieldConfigRepository)
		{
            this._uniqueIdGenerator = uniqueIdGenerator;
            this._flowFieldConfigRepository = flowFieldConfigRepository;
        }

        public async Task Handle(FieldConfigCreateEvent notification, CancellationToken cancellationToken)
        {
            var fieldConfigs = this._flowFieldConfigRepository.Table.FirstOrDefault(p => p.Name == notification.Name && p.WorkFlowModuleSceneConfigId == notification.WorkFlowModuleSceneConfigId);
            fieldConfigs.ThrowIf(p => p != null, "已存在相同字段名，不可重复添加");

            var fieldConfig = FieldConfig.New();
            notification.Id = this._uniqueIdGenerator.NewId();
            fieldConfig.Load(notification);
            fieldConfig.CheckExecuteMethodImplemented();
            await this._flowFieldConfigRepository.InsertAsync(fieldConfig);
        }
    }
}

