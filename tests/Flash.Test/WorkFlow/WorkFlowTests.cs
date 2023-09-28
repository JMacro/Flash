using Flash.AspNetCore.WorkFlow.Application.Abastracts.Services;
using Flash.AspNetCore.WorkFlow.Application.Dtos.FlowConfigs;
using Flash.AspNetCore.WorkFlow.Domain;
using Flash.AspNetCore.WorkFlow.Domain.DO;
using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using Flash.Extensions;
using Flash.Test.StartupTests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test.WorkFlow
{
    [TestFixture]
    public class WorkFlowTests : BaseTest<WorkFlowStartupTest>
    {
        [Test]
        public void InitBusinessModule()
        {
            var logger = this.ServiceProvider.GetService<ILogger<WorkFlowTests>>();


            var flowConfigService = this.ServiceProvider.GetService<Flash.AspNetCore.WorkFlow.Domain.Abastracts.Services.IFlowConfigService>();
            flowConfigService.Init(new List<AspNetCore.WorkFlow.Domain.DO.InitFlowConfigRequestDO>
            {
                new InitFlowConfigRequestDO { Id = 1,Name="业务模块1",Type = EWorkFlowConfigType.ApprovalCenter,SubType = EWorkFlowConfigSubType.BusinessModule,ClassType = 1,ClassSubType = 1,ParentId = 0,Remark = "备注" }.AddFieldConfig("Test","FieldName1","DisplayName1",typeof(TestCommandHandle)).AddFieldConfig("Test","FieldName2","DisplayName2",typeof(TestCommandHandle)),
                new InitFlowConfigRequestDO { Id = 11,Name="业务模块1->业务场景1",Type = EWorkFlowConfigType.ApprovalCenter,SubType = EWorkFlowConfigSubType.BusinessScenario,ClassType = 1,ClassSubType = 1,ParentId = 1,Remark = "备注" },
                new InitFlowConfigRequestDO { Id = 2,Name="业务模块2",Type = EWorkFlowConfigType.ApprovalCenter,SubType = EWorkFlowConfigSubType.BusinessModule,ClassType = 2,ClassSubType = 2,ParentId = 0,Remark = "备注" },
                new InitFlowConfigRequestDO { Id = 3,Name="业务模块3",Type = EWorkFlowConfigType.ApprovalCenter,SubType = EWorkFlowConfigSubType.BusinessModule,ClassType = 3,ClassSubType = 3,ParentId = 0,Remark = "备注" },
                new InitFlowConfigRequestDO { Id = 4,Name="业务模块4",Type = EWorkFlowConfigType.ApprovalCenter,SubType = EWorkFlowConfigSubType.BusinessModule,ClassType = 4,ClassSubType = 4,ParentId = 0,Remark = "备注" },
            }).ConfigureAwait(false).GetAwaiter().GetResult();

            var businessModule = this.ServiceProvider.GetService<IFlowConfigService>();


            //businessModule.Init(() =>
            //{
            //    return new List<FlowConfigRequestDto>
            //    {
            //        new FlowConfigRequestDto { Id = 1,Name="业务模块1",Type = EWorkFlowConfigType.ApprovalCenter,SubType = EWorkFlowConfigSubType.BusinessModule,ClassType = 1,ClassSubType = 1,ParentId = 0,Remark = "备注" }.AddFieldConfig("Test","FieldName1","DisplayName1",typeof(TestCommandHandle)).AddFieldConfig("Test","FieldName2","DisplayName2",typeof(TestCommandHandle)),
            //        new FlowConfigRequestDto { Id = 11,Name="业务模块1->业务场景1",Type = EWorkFlowConfigType.ApprovalCenter,SubType = EWorkFlowConfigSubType.BusinessScenario,ClassType = 1,ClassSubType = 1,ParentId = 1,Remark = "备注" },
            //        new FlowConfigRequestDto { Id = 2,Name="业务模块2",Type = EWorkFlowConfigType.ApprovalCenter,SubType = EWorkFlowConfigSubType.BusinessModule,ClassType = 2,ClassSubType = 2,ParentId = 0,Remark = "备注" },
            //        new FlowConfigRequestDto { Id = 3,Name="业务模块3",Type = EWorkFlowConfigType.ApprovalCenter,SubType = EWorkFlowConfigSubType.BusinessModule,ClassType = 3,ClassSubType = 3,ParentId = 0,Remark = "备注" },
            //        new FlowConfigRequestDto { Id = 4,Name="业务模块4",Type = EWorkFlowConfigType.ApprovalCenter,SubType = EWorkFlowConfigSubType.BusinessModule,ClassType = 4,ClassSubType = 4,ParentId = 0,Remark = "备注" },
            //    };
            //});

            //var list = businessModule.GetFlowConfigs();
            //logger.LogInformation(list.ToJson());

            //var workFlowFieldConfigService = this.ServiceProvider.GetService<IFlowFieldConfigService>();

            //workFlowFieldConfigService.TestFieldExecuteMethod(1706948982380429607);
            //var fields = workFlowFieldConfigService.GetFields(1).ConfigureAwait(false).GetAwaiter().GetResult();
            //logger.LogInformation(fields.ToJson());
        }

        [Test]
        public void WorkFlowTypeInit()
        {
            var saveWorkFlowTypeDomainService = new SaveFlowTypeDomainContext(new AspNetCore.WorkFlow.Domain.DO.SaveFlowTypeRequestDO
            {
                ParentId = 11,
                Name = "业务模块1->业务场景1->工作流名称1",
                FieldIds = new List<long> { 1706948982380429607 }
            });
            saveWorkFlowTypeDomainService
                .SaveFlowTypeValidator().ConfigureAwait(false).GetAwaiter().GetResult()
                .SaveFormType().ConfigureAwait(false).GetAwaiter().GetResult();


            //var workFlowType = new FlowType();
            //workFlowType.Init(new WorkFlowTypeData
            //{
            //    Id = "1",
            //    Nodes = new List<NodeTypeInfoData>
            //    {
            //        new AutoHandleNodeTypeInfoData
            //        {
            //            Id ="11",
            //            Name = "11",
            //            IsRoot = true,
            //            SubNodeId = new List<string>{ "12" }
            //        },
            //        new AutoHandleNodeTypeInfoData
            //        {
            //            Id ="12",
            //            Name = "12",
            //            IsRoot = false
            //        }
            //    }
            //});
        }
    }

    public class TestCommand : IRequest<List<EWorkFlowFieldDataType>>
    {

    }

    public class TestCommandHandle : IRequestHandler<TestCommand, List<EWorkFlowFieldDataType>>
    {
        public Task<List<EWorkFlowFieldDataType>> Handle(TestCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Enum.GetValues<EWorkFlowFieldDataType>().ToList());
        }
    }
}

