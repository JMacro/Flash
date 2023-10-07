using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Repositories;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs;
using MediatR;

namespace Flash.AspNetCore.WorkFlow.Domain.Commands.FlowConfigs.UpdateFlowConfig
{
    internal class UpdateFlowConfigCommandHandler : IRequestHandler<UpdateFlowConfigCommand>
    {
        private readonly IFlowConfigRepository _flowConfigRepository;

        public UpdateFlowConfigCommandHandler(IFlowConfigRepository flowConfigRepository)
		{
            this._flowConfigRepository = flowConfigRepository;
        }

        public Task Handle(UpdateFlowConfigCommand request, CancellationToken cancellationToken)
        {
            var entitys = request.FlowConfigs.Select(p => FlowConfig.Update(p, p.FieldConfigs));
            this._flowConfigRepository.Update(entitys.ToArray());

            return Task.CompletedTask;
        }
    }
}

