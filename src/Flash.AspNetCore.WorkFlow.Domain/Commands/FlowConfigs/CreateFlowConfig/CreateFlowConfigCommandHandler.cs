using AutoMapper;
using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Repositories;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs;
using Flash.AspNetCore.WorkFlow.Infrastructure.PO;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.AspNetCore.WorkFlow.Domain.Commands.FlowConfigs.CreateFlowConfig
{
    internal class CreateFlowConfigCommandHandler : IRequestHandler<CreateFlowConfigCommand>
    {
        private readonly IMapper _mapper;
        private readonly IFlowConfigRepository _flowConfigRepository;

        public CreateFlowConfigCommandHandler(IMapper mapper, IFlowConfigRepository flowConfigRepository)
        {
            this._mapper = mapper;
            this._flowConfigRepository = flowConfigRepository;
        }

        public async Task Handle(CreateFlowConfigCommand request, CancellationToken cancellationToken)
        {
            var flowConfigs = await this._flowConfigRepository.Table.ToListAsync();

            var insertFlowConfigCreateDatas = request.FlowConfigs.Select(p => p.Id).Except(flowConfigs.Select(p => p.Id)).ToList();
            var updateFlowConfigCreateDatas = request.FlowConfigs.Select(p => p.Id).Intersect(flowConfigs.Select(p => p.Id)).ToList();
            var deleteFlowConfigCreateDatas = flowConfigs.Select(p => p.Id).Except(request.FlowConfigs.Select(p => p.Id)).ToList();

            if (insertFlowConfigCreateDatas.Any())
            {
                var entitys = request.FlowConfigs.Where(p => insertFlowConfigCreateDatas.Contains(p.Id)).Select(p => FlowConfig.Create(p, request.FieldConfigs.Where(f => f.WorkFlowModuleSceneConfigId == p.Id).ToList())).ToList();
                await this._flowConfigRepository.InsertAsync(entitys.ToArray());
            }

            //if (updateFlowConfigCreateDatas.Any())
            //{
            //    var updateModel = new List<FlowConfig>();
            //    updateFlowConfigCreateDatas.ForEach(id =>
            //    {
            //        var flowConfig = flowConfigs.FirstOrDefault(p => p.Id == id);
            //        var dto = request.FlowConfigs.FirstOrDefault(p => p.Id == id);
            //        if (flowConfig != null &&
            //            (flowConfig.ParentId != dto.ParentId ||
            //            flowConfig.Name != dto.Name ||
            //            flowConfig.Type != dto.Type ||
            //            flowConfig.SubType != dto.SubType ||
            //            flowConfig.ClassType != dto.ClassType ||
            //            flowConfig.ClassSubType != dto.ClassSubType ||
            //            flowConfig.Remark != dto.Remark
            //            )
            //        )
            //        {
            //            flowConfig.ParentId = dto.ParentId;
            //            flowConfig.Name = dto.Name;
            //            flowConfig.Type = dto.Type;
            //            flowConfig.SubType = dto.SubType;
            //            flowConfig.ClassType = dto.ClassType;
            //            flowConfig.ClassSubType = dto.ClassSubType;
            //            flowConfig.Remark = dto.Remark;

            //            updateModel.Add(flowConfig);
            //        }
            //    });

            //    if (updateModel.Any()) _flowConfigRepository.Update(updateModel.ToArray());
            //}
        }
    }
}
