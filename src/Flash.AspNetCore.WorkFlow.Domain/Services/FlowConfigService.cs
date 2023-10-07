using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Repositories;
using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Services;
using Flash.AspNetCore.WorkFlow.Domain.Commands.FlowConfigs.CreateFlowConfig;
using Flash.AspNetCore.WorkFlow.Domain.Commands.FlowConfigs.UpdateFlowConfig;
using Flash.AspNetCore.WorkFlow.Domain.DO;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FieldConfigs;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs;
using Flash.AspNetCore.WorkFlow.Infrastructure;
using Flash.AspNetCore.WorkFlow.Infrastructure.PO;
using Flash.Extensions.ORM.EntityFrameworkCore;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flash.AspNetCore.WorkFlow.Domain.Services
{
    public sealed class FlowConfigService : IFlowConfigService
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IFlowConfigRepository _flowConfigRepository;
        private readonly IWorkFlowUnitOfWork _unitOfWork;

        public FlowConfigService(
            IMapper mapper,
            IMediator mediator,
            IFlowConfigRepository flowConfigRepository,
            IWorkFlowUnitOfWork unitOfWork)
        {
            this._mapper = mapper;
            this._mediator = mediator;
            this._flowConfigRepository = flowConfigRepository;
            this._unitOfWork = unitOfWork;
        }

        public async Task Init(List<InitFlowConfigRequestDO> initFlowConfigs)
        {
            var flowConfigs = await this._flowConfigRepository.Table.ToListAsync();

            var insertFlowConfigIds = initFlowConfigs.Select(p => p.Id).Except(flowConfigs.Select(p => p.Id)).ToList();
            var updateFlowConfigIds = initFlowConfigs.Select(p => p.Id).Intersect(flowConfigs.Select(p => p.Id)).ToList();
            var deleteFlowConfigIds = flowConfigs.Select(p => p.Id).Except(initFlowConfigs.Select(p => p.Id)).ToList();

            if (insertFlowConfigIds.Any())
            {
                var insertDatas = initFlowConfigs.Where(p => insertFlowConfigIds.Contains(p.Id)).ToList();
                var insertFlowConfigs = this._mapper.Map<List<FlowConfigSaveData>>(insertDatas);
                await this._mediator.Send(new CreateFlowConfigCommand
                {
                    FlowConfigs = insertFlowConfigs
                });
            }

            if (updateFlowConfigIds.Any())
            {
                var updateDatas = initFlowConfigs.Where(p => updateFlowConfigIds.Contains(p.Id)).ToList();
                var updateFlowConfigs = this._mapper.Map<List<FlowConfigSaveData>>(updateDatas);
                await this._mediator.Send(new UpdateFlowConfigCommand
                {
                    FlowConfigs = updateFlowConfigs
                });
            }



            //var flowConfig = new FlowConfig();
            //if (insertFlowConfigCreateDatas.Any())
            //{
            //    var entitys = flowConfigCreateDatas.Where(p => insertFlowConfigCreateDatas.Contains(p.Id));
            //    _flowConfigRepository.Insert(entitys.ToArray());
            //}


            //foreach (var initFlowConfig in initFlowConfigs)
            //{
            //    var flowConfigCreateData = this._mapper.Map<FlowConfigCreateData>(initFlowConfig);

            //}

            //var flowConfig = new FlowConfig();
            //flowConfig.Create(new FlowConfigCreateData { });

            var dd = this._unitOfWork.SaveChanges();
        }
    }
}
