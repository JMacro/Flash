using Flash.AspNetCore.WorkFlow.Application.Dtos.FlowFieldConfigs;
using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Flash.AspNetCore.WorkFlow.Application.Dtos.FlowConfigs
{
    /// <summary>
    /// 工作流模块与场景初始化
    /// </summary>
    public class FlowConfigRequestDto
    {
        public long Id { get; set; }
        /// <summary>
        /// 父Id
        /// </summary>
        [Description("父Id")]
        public long ParentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }
        /// <summary>
        /// 流程配置类型
        /// </summary>
        [Description("流程配置类型")]
        public EWorkFlowConfigType Type { get; set; }
        /// <summary>
        /// 流程配置类型
        /// </summary>
        [Description("流程配置子类型")]
        public EWorkFlowConfigSubType SubType { get; set; }
        /// <summary>
        /// 分类类型
        /// </summary>
        [Description("分类类型")]
        public short ClassType { get; set; }
        /// <summary>
        /// 分类子类型
        /// </summary>
        [Description("分类子类型")]
        public short ClassSubType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; } = "";
        /// <summary>
        /// 字段配置
        /// </summary>
        [Description("字段配置")]
        public List<FlowFieldConfigRequestDto> FieldConfigs { get; set; } = new List<FlowFieldConfigRequestDto>();
    }

    public static class WorkFlowModuleSceneConfigInitDtoExtersion
    {
        public static FlowConfigRequestDto AddFieldConfig(this FlowConfigRequestDto moduleSceneConfigInitDto,
            string tableName,
            string fieldName,
            string displayName,
            Type executeCommandHandle = null,
            string resultType = nameof(String),
            string unit = "",
            bool isSingleSelect = true,
            string stringKeyItems = "",
            EWorkFlowFieldDataType type = EWorkFlowFieldDataType.String)
        {
            moduleSceneConfigInitDto.FieldConfigs.Add(new FlowFieldConfigRequestDto
            {
                Name = fieldName,
                TableName = tableName,
                DisplayName = displayName,
                ExecuteMethod = executeCommandHandle != null ? $"{executeCommandHandle.FullName},{executeCommandHandle.Assembly.GetName().Name}" : "",
                ResultType = resultType,
                Unit = unit,
                Enable = true,
                IsSingleSelect = isSingleSelect,
                Sort = 0,
                Type = type
            });
            return moduleSceneConfigInitDto;
        }
    }
}
