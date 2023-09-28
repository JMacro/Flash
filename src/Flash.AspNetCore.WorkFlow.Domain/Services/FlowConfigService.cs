using AutoMapper;
using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Repositories;
using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Services;
using Flash.AspNetCore.WorkFlow.Domain.Commands.FlowConfigs.CreateFlowConfig;
using Flash.AspNetCore.WorkFlow.Domain.DO;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FieldConfigs;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs;
using Flash.Extensions.ORM.EntityFrameworkCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.AspNetCore.WorkFlow.Domain.Services
{
    public sealed class FlowConfigService : IFlowConfigService
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IFlowConfigRepository _flowConfigRepository;
        private readonly IUnitOfWork<WorkFlowDbContext> _unitOfWork;

        public FlowConfigService(
            IMapper mapper,
            IMediator mediator,
            IFlowConfigRepository flowConfigRepository,
            IUnitOfWork<WorkFlowDbContext> unitOfWork)
        {
            this._mapper = mapper;
            this._mediator = mediator;
            this._flowConfigRepository = flowConfigRepository;
            this._unitOfWork = unitOfWork;
        }

        public async Task Init(List<InitFlowConfigRequestDO> initFlowConfigs)
        {
            var flowConfigCreateDatas = this._mapper.Map<List<FlowConfigCreateData>>(initFlowConfigs);
            var flowFieldConfigs = initFlowConfigs.SelectMany(p => p.FieldConfigs.Select(s => new FieldConfigSaveData
            {
                WorkFlowModuleSceneConfigId = p.Id,
                Name = s.Name,
                Type = s.Type,
                DisplayName = s.DisplayName,
                Unit = s.Unit,
                IsSingleSelect = s.IsSingleSelect,
                Enable = s.Enable,
                ExecuteMethod = s.ExecuteMethod,
                ResultType = s.ResultType,
                Sort = s.Sort,
            })).ToList();

            await this._mediator.Send(new CreateFlowConfigCommand { FlowConfigs = flowConfigCreateDatas, FieldConfigs = flowFieldConfigs });




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
