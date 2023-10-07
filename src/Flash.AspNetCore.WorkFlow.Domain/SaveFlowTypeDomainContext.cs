using Flash.AspNetCore.WorkFlow.Domain.Abastracts.Repositories;
using Flash.AspNetCore.WorkFlow.Domain.Commands.FlowForms.DefineFlowForm;
using Flash.AspNetCore.WorkFlow.Domain.Commands.FlowForms.GetFlowFormDetial;
using Flash.AspNetCore.WorkFlow.Domain.DO;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FieldConfigs;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs;
using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using Flash.AspNetCore.WorkFlow.Infrastructure.PO;
using Flash.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flash.AspNetCore.WorkFlow.Domain
{
    public class SaveFlowTypeDomainContext
    {
        private readonly IFlowTypeRepository _workFlowTypeRepository;
        private readonly IFlowConfigRepository _workFlowConfigRepository;
        private readonly IFlowFieldConfigRepository _workFlowFieldConfigRepository;
        private readonly IMediator _mediator;

        /// <summary>
        /// 请求入参
        /// </summary>
        public SaveFlowTypeRequestDO RequestData { get; private set; }

        /// <summary>
        /// 业务场景配置
        /// </summary>
        public FlowConfig BusinessScenarioConfig { get; private set; }

        /// <summary>
        /// 流程配置
        /// </summary>
        public FlowConfig FlowTypeConfig { get; private set; }

        /// <summary>
        /// 表单类型Id
        /// </summary>
        public long FormTypeId { get; private set; }

        /// <summary>
        /// 流程类型Id
        /// </summary>
        public long? FlowTypeId { get; private set; }

        /// <summary>
        /// 字段
        /// </summary>
        public IList<FieldConfig> Fields { get; private set; }

        /// <summary>
        /// 表单数据
        /// </summary>
        public IDictionary<string, string> FormData { get; private set; }

        public SaveFlowTypeDomainContext(SaveFlowTypeRequestDO request)
        {
            this.RequestData = request;
            this._workFlowTypeRepository = MicrosoftContainer.Instance.GetService<IFlowTypeRepository>();
            this._workFlowConfigRepository = MicrosoftContainer.Instance.GetService<IFlowConfigRepository>();
            this._workFlowFieldConfigRepository = MicrosoftContainer.Instance.GetService<IFlowFieldConfigRepository>();
            this._mediator = MicrosoftContainer.Instance.GetService<IMediator>();

            this.Fields = GetFields(RequestData.FieldIds.ToArray());
            this.FormData = this.Fields.Select(p => new { Key = $"{{{p.TableName}:{p.Name}}}", Value = p.Type })
                .ToDictionary(x => x.Key, x => FieldDataTypeMapString(x.Value));
        }

        /// <summary>
        /// 保存流程类型验证器
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<SaveFlowTypeDomainContext> SaveFlowTypeValidator()
        {
            var workFlowConfigQueryable = this._workFlowConfigRepository.TableNoTracking.Where(p => p.Type == Infrastructure.Enums.EWorkFlowConfigType.ApprovalCenter);

            //业务场景
            var businessScenarioConfig = await workFlowConfigQueryable.FirstOrDefaultAsync(p => p.SubType == EWorkFlowConfigSubType.BusinessScenario && p.Id == RequestData.ParentId);
            businessScenarioConfig.ThrowIfNull("业务场景不存在,无法保存");

            // 流程类型配置
            var flowTypeConfigs = await workFlowConfigQueryable.Where(p => p.SubType == EWorkFlowConfigSubType.FlowType).ToListAsync();
            //查找存在同名的流程类型
            var flowTypeConfig = flowTypeConfigs.FirstOrDefault(p => p.ParentId == RequestData.ParentId && p.ObjectId != RequestData.Id && p.Name == RequestData.Name);
            flowTypeConfig.ThrowIf(p => p != null, "当前名称已存在,请修改名称");

            flowTypeConfig = flowTypeConfigs.FirstOrDefault(p => p.ObjectId == RequestData.Id);
            if (flowTypeConfig == null && flowTypeConfigs.Any(p => p.ParentId == businessScenarioConfig.Id))
            {
                BusinessException.Throw("当前选择的业务场景已创建流程,无法重复操作");
            }

            return SetBusinessScenarioConfig(businessScenarioConfig)
                .SetFlowTypeConfig(flowTypeConfig)
                .SetFormTypeId(businessScenarioConfig.ObjectId)
                .SetFlowTypeId(flowTypeConfig?.ObjectId);
        }

        /// <summary>
        /// 保存表单类型
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<SaveFlowTypeDomainContext> SaveFormType()
        {
            if (FormTypeId > 0)
            {
                await this._mediator.Send(new GetFlowFormDetialQuery
                {
                    Id = FormTypeId
                });
            }
            

            await this._mediator.Send(new DefineFlowFormCommand
            {
                Name = $"{RequestData.Name}表单",
                Fields = FormData
            });

            return this;
        }

        public void SaveFlowType()
        {
            var fields = GetFields(RequestData.FieldIds.ToArray());
            var formData = fields.Select(p => new { Key = $"{{{p.TableName}:{p.Name}}}", Value = p.Type })
            .ToDictionary(x => x.Key, x => FieldDataTypeMapString(x.Value));
        }

        private List<FieldConfig> GetFields(params long[] fieldIds)
        {
            return this._workFlowFieldConfigRepository.TableNoTracking.Where(p => fieldIds.Contains(p.Id)).ToList();
        }

        private SaveFlowTypeDomainContext SetBusinessScenarioConfig(FlowConfig businessScenarioConfig)
        {
            this.BusinessScenarioConfig = businessScenarioConfig;
            return this;
        }
        private SaveFlowTypeDomainContext SetFlowTypeConfig(FlowConfig flowTypeConfig)
        {
            FlowTypeConfig = flowTypeConfig;
            return this;
        }

        private SaveFlowTypeDomainContext SetFormTypeId(long formTypeId)
        {
            FormTypeId = formTypeId;
            return this;
        }

        private SaveFlowTypeDomainContext SetFlowTypeId(long? flowTypeId)
        {
            FlowTypeId = flowTypeId;
            return this;
        }

        /// <summary>
        /// 字段数据类型映射
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        private string FieldDataTypeMapString(EWorkFlowFieldDataType dataType)
            => dataType switch
            {
                EWorkFlowFieldDataType.String => nameof(String),
                EWorkFlowFieldDataType.Decimal => nameof(Decimal),
                EWorkFlowFieldDataType.Int => nameof(Int32),
                EWorkFlowFieldDataType.Dropdown => nameof(Int32),
                EWorkFlowFieldDataType.Date => nameof(DateTime),
                _ => throw new BusinessException("数据类型异常")
            };
    }
}
