using Flash.AspNetCore.WorkFlow.Application.Abastracts.Services;
using Flash.AspNetCore.WorkFlow.Application.Dtos.FlowConfigs;
using Flash.AspNetCore.WorkFlow.Application.Dtos.FlowFieldConfigs;
using Flash.AspNetCore.WorkFlow.Domain;
using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Repositories;
using Flash.AspNetCore.WorkFlow.Infrastructure;
using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using Flash.AspNetCore.WorkFlow.Infrastructure.PO;
using Flash.Extensions;
using Flash.Extensions.ORM.EntityFrameworkCore;
using Flash.Extensions.UidGenerator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flash.AspNetCore.WorkFlow.Application.Services
{
    public sealed class FlowConfigService : IFlowConfigService
    {
        private readonly IUniqueIdGenerator _uniqueIdGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFlowConfigRepository _flowConfigRepository;
        private readonly IFlowFieldConfigService _workFlowFieldConfigService;

        public FlowConfigService()
        {
            _uniqueIdGenerator = MicrosoftContainer.Instance.GetService<IUniqueIdGenerator>();
            _unitOfWork = MicrosoftContainer.Instance.GetService<IUnitOfWork<WorkFlowDbContext>>();
            _flowConfigRepository = MicrosoftContainer.Instance.GetService<IFlowConfigRepository>();
            _workFlowFieldConfigService = MicrosoftContainer.Instance.GetService<IFlowFieldConfigService>();
        }

        public void Init(Func<IEnumerable<FlowConfigRequestDto>> func)
        {
            var list = func?.Invoke();
            if (list != null) Adds(list);
        }

        public List<FlowConfigTreeResponseDto> GetFlowConfigs()
        {
            var result = _flowConfigRepository.Table.Select(p => new FlowConfigTreeResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                ParentId = p.ParentId,
                Type = p.Type,
                SubType = p.SubType,
                ClassType = p.ClassType,
                ClassSubType = p.ClassSubType,
                Remark = p.Remark,
                IsDelete = p.IsDelete,
            }).ToList();
            var parentConfigs = result.Where(p => p.ParentId == 0).ToList();
            RecursionFlowConfig(result, parentConfigs);
            return parentConfigs;

            void RecursionFlowConfig(List<FlowConfigTreeResponseDto> allList, List<FlowConfigTreeResponseDto> parentList)
            {
                foreach (var item in parentList)
                {
                    var childrens = allList.Where(p => p.ParentId == item.Id);
                    var childrenIds = childrens.Select(p => p.Id);
                    var existIds = allList.Where(p => p.SubType == EWorkFlowConfigSubType.FlowType && childrenIds.Contains(p.ParentId))
                        .Select(p => p.ParentId);

                    foreach (var children in childrens)
                    {
                        children.Display = !existIds.Contains(children.Id);
                    }

                    item.Children = childrens.ToList();
                    RecursionFlowConfig(allList, item.Children);
                }
            }
        }

        private void Adds(IEnumerable<FlowConfigRequestDto> model)
        {
            #region WorkFlowInitConfig配置
            var flowConfigs = _flowConfigRepository.Table.ToList();
            var workFlowInitConfigs = model.Select(p => new FlowConfigPO
            {
                Id = p.Id,
                ParentId = p.ParentId,
                Name = p.Name,
                ObjectId = 0,
                Type = p.Type,
                SubType = p.SubType,
                ClassType = p.ClassType,
                ClassSubType = p.ClassSubType,
                Remark = p.Remark,
            }).ToList();

            var insertWorkFlowInitConfigList = workFlowInitConfigs.Select(p => p.Id).Except(flowConfigs.Select(p => p.Id));
            var updateWorkFlowInitConfigList = workFlowInitConfigs.Select(p => p.Id).Intersect(flowConfigs.Select(p => p.Id));
            var deleteWorkFlowInitConfigList = flowConfigs.Select(p => p.Id).Except(workFlowInitConfigs.Select(p => p.Id));

            if (insertWorkFlowInitConfigList.Any())
            {
                var entitys = workFlowInitConfigs.Where(p => insertWorkFlowInitConfigList.Contains(p.Id));
                _flowConfigRepository.Insert(entitys.ToArray());
            }
            if (updateWorkFlowInitConfigList.Any())
            {
                var updateModel = new List<FlowConfigPO>();
                updateWorkFlowInitConfigList.ForEach(id =>
                {
                    var flowConfig = flowConfigs.FirstOrDefault(p => p.Id == id);
                    var dto = workFlowInitConfigs.FirstOrDefault(p => p.Id == id);
                    if (flowConfig != null &&
                        (flowConfig.ParentId != dto.ParentId ||
                        flowConfig.Name != dto.Name ||
                        flowConfig.Type != dto.Type ||
                        flowConfig.SubType != dto.SubType ||
                        flowConfig.ClassType != dto.ClassType ||
                        flowConfig.ClassSubType != dto.ClassSubType ||
                        flowConfig.Remark != dto.Remark
                        )
                    )
                    {
                        flowConfig.ParentId = dto.ParentId;
                        flowConfig.Name = dto.Name;
                        flowConfig.Type = dto.Type;
                        flowConfig.SubType = dto.SubType;
                        flowConfig.ClassType = dto.ClassType;
                        flowConfig.ClassSubType = dto.ClassSubType;
                        flowConfig.Remark = dto.Remark;

                        updateModel.Add(flowConfig);
                    }
                });

                if (updateModel.Any()) _flowConfigRepository.Update(updateModel.ToArray());
            }
            if (deleteWorkFlowInitConfigList.Any()) _flowConfigRepository.Delete(flowConfigs.Where(p => deleteWorkFlowInitConfigList.Contains(p.Id)).ToArray());
            #endregion

            #region WorkFlowFieldConfig配置
            var workFlowFieldConfigs = model.SelectMany(p => p.FieldConfigs.Select(s => new FlowFieldConfigRequestDto
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
            _workFlowFieldConfigService.SaveData(workFlowFieldConfigs);
            #endregion

            _unitOfWork.SaveChanges();
        }
    }
}
