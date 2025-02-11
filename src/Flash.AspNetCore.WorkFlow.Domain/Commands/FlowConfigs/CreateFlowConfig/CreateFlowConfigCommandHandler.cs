using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Repositories;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs;
using MediatR;

namespace Flash.AspNetCore.WorkFlow.Domain.Commands.FlowConfigs.CreateFlowConfig
{
    internal class CreateFlowConfigCommandHandler : IRequestHandler<CreateFlowConfigCommand>
    {
        private readonly IFlowConfigRepository _flowConfigRepository;

        public CreateFlowConfigCommandHandler(IFlowConfigRepository flowConfigRepository)
        {
            this._flowConfigRepository = flowConfigRepository;
        }

        public async Task Handle(CreateFlowConfigCommand request, CancellationToken cancellationToken)
        {
            var entitys = request.FlowConfigs.Select(p => FlowConfig.Create(p, p.FieldConfigs));
            await this._flowConfigRepository.InsertAsync(entitys.ToArray());
        }
    }
}
