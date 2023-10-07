using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Flash.AspNetCore.WorkFlow.Application.Abastracts.Services;
using Flash.AspNetCore.WorkFlow.Application.Dtos.FlowFieldConfigs;
using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Repositories;
using Flash.AspNetCore.WorkFlow.Infrastructure;
using Flash.AspNetCore.WorkFlow.Infrastructure.PO;
using Flash.Core;
using Flash.Extensions;
using Flash.Extensions.ORM.EntityFrameworkCore;
using Flash.Extensions.UidGenerator;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.AspNetCore.WorkFlow.Application.Services
{
    public sealed class FlowFieldConfigService : IFlowFieldConfigService
    {
        private static object _Lock = new object();
        private static ConcurrentDictionary<string, IBaseRequest> CacheCommandHandleTypes = new ConcurrentDictionary<string, IBaseRequest>();
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IUniqueIdGenerator _uniqueIdGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFlowFieldConfigRepository _workFlowFieldConfigRepository;

        public FlowFieldConfigService()
        {
            this._mediator = MicrosoftContainer.Instance.GetService<IMediator>();
            this._mapper = MicrosoftContainer.Instance.GetService<IMapper>();
            this._uniqueIdGenerator = MicrosoftContainer.Instance.GetService<IUniqueIdGenerator>();
            this._unitOfWork = MicrosoftContainer.Instance.GetService<IUnitOfWork<WorkFlowDbContext>>();
            this._workFlowFieldConfigRepository = MicrosoftContainer.Instance.GetService<IFlowFieldConfigRepository>();
        }

        public void SaveData(List<FlowFieldConfigRequestDto> workFlowFields)
        {
            var fieldConfigs = _workFlowFieldConfigRepository.Table.ToList();
            var workFlowFieldConfigs = workFlowFields.Select(s => new FlowFieldConfigPO
            {
                Id = _uniqueIdGenerator.NewId(),
                WorkFlowModuleSceneConfigId = s.WorkFlowModuleSceneConfigId,
                Name = s.Name,
                Type = s.Type,
                DisplayName = s.DisplayName,
                Unit = s.Unit,
                IsSingleSelect = s.IsSingleSelect,
                Enable = s.Enable,
                ExecuteMethod = s.ExecuteMethod,
                ResultType = s.ResultType,
                Sort = s.Sort,
            }).ToList();

            workFlowFieldConfigs.ForEach(item =>
            {
                CheckExecuteMethodImplemented(item.ExecuteMethod);
            });

            var existFields = fieldConfigs.Where(p =>
                    workFlowFieldConfigs.Any(k => k.Name == p.Name && k.WorkFlowModuleSceneConfigId == p.WorkFlowModuleSceneConfigId))
                .ToList();

            if (!existFields.Any())
            {
                _workFlowFieldConfigRepository.Insert(workFlowFieldConfigs.ToArray());
            }
            else
            {
                foreach (var field in existFields)
                {
                    var targetField = workFlowFieldConfigs.First(n => n.Name == field.Name && n.WorkFlowModuleSceneConfigId == field.WorkFlowModuleSceneConfigId);
                    field.WorkFlowModuleSceneConfigId = targetField.WorkFlowModuleSceneConfigId;
                    field.Name = targetField.Name;
                    field.DisplayName = targetField.DisplayName;
                    field.ExecuteMethod = targetField.ExecuteMethod;
                    field.ResultType = targetField.ResultType;
                    field.Unit = targetField.Unit;
                    field.IsSingleSelect = targetField.IsSingleSelect;
                    field.Type = targetField.Type;
                }
                _workFlowFieldConfigRepository.Update(existFields.ToArray());

                // 提交后少了,删除原表多余的
                var remoteFields = fieldConfigs.Where(p =>
                    workFlowFieldConfigs.FirstOrDefault(x => x.WorkFlowModuleSceneConfigId == p.WorkFlowModuleSceneConfigId && x.Name == p.Name) == null);
                if (remoteFields.Any())
                {
                    _workFlowFieldConfigRepository.Delete(remoteFields.ToArray());
                }

                // 提交后多了,原表添加新记录
                var addFields = workFlowFieldConfigs.Where(p =>
                    fieldConfigs.FirstOrDefault(x => x.WorkFlowModuleSceneConfigId == p.WorkFlowModuleSceneConfigId && x.Name == p.Name) == null);

                if (addFields.Any())
                {
                    _workFlowFieldConfigRepository.Insert(workFlowFieldConfigs.ToArray());//批量保存
                }
            }

            _unitOfWork.SaveChanges();
        }

        public async Task<List<FlowFieldConfigResponseDto>> GetFields(long workFlowModuleSceneConfigId)
        {
            var list = _workFlowFieldConfigRepository.TableNoTracking.Where(p => p.WorkFlowModuleSceneConfigId == workFlowModuleSceneConfigId).ToList();
            var responseData = _mapper.Map<List<FlowFieldConfigResponseDto>>(list);
            foreach (var item in list)
            {
                var result = CheckExecuteMethodImplemented(item.ExecuteMethod);
                if (result.Result)
                {
                    var info = responseData.FirstOrDefault(p => p.Id == item.Id);
                    if (info != null)
                    {
                        info.Values = await _mediator.Send(result.BaseRequest);
                    }
                }
            }
            return responseData;
        }

        public async void TestFieldExecuteMethod(long fieldId)
        {
            var entity = await _workFlowFieldConfigRepository.GetByIdAsync(fieldId);
            if (entity == null) throw new BusinessException("未找到工作流字段配置信息");

            var result = CheckExecuteMethodImplemented(entity.ExecuteMethod);
            if (result.Result)
            {
                await _mediator.Send(result.BaseRequest);
            }
        }

        /// <summary>
        /// 检查字段值获取的执行方法是否异常
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        private (bool Result, IBaseRequest BaseRequest) CheckExecuteMethodImplemented(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return (false, null);

            var executeCommandHandleType = EntityTypeCaches.TryGetOrAddByType(fullName);
            if (executeCommandHandleType == null) throw new BusinessException($"无法找到类型{fullName}");

            var commandHandleType = executeCommandHandleType.GetInterfaces().FirstOrDefault(p => p.HasImplementedRawGeneric(typeof(IRequestHandler<,>)));
            if (commandHandleType == null)
                throw new BusinessException($"{executeCommandHandleType.Name}未继承{typeof(IRequestHandler<,>).FullName}");

            var executeCommandType = commandHandleType.GetGenericArguments().First();
            lock (_Lock)
            {
                var result = CacheCommandHandleTypes.TryGetValue(fullName, out IBaseRequest objCommandHandle);
                if (!result)
                {
                    if (Activator.CreateInstance(executeCommandType) is IBaseRequest baseRequest)
                    {
                        objCommandHandle = baseRequest;
                        CacheCommandHandleTypes.TryAdd(fullName, objCommandHandle);
                        return (true, objCommandHandle);
                    }
                }
                return (result, objCommandHandle);
            }
        }
    }
}
