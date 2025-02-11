using Flash.AspNetCore.WorkFlow.Domain.Entitys.FieldConfigs;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs;
using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using Flash.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.DO
{
    [AutoMapperTo(typeof(FlowConfigSaveData))]
    public class InitFlowConfigRequestDO
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
        public List<InitFlowFieldConfigRequestDO> FieldConfigs { get; set; } = new List<InitFlowFieldConfigRequestDO>();
    }

    [AutoMapperTo(typeof(FieldConfigSaveData))]
    public class InitFlowFieldConfigRequestDO
    {
        /// <summary>
        /// 字段名
        /// </summary>
        [Description("字段名")]
        public string Name { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [Description("表名")]
        public string TableName { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        [Description("数据类型")]
        public EWorkFlowFieldDataType Type { get; set; }
        /// <summary>
        /// 字段显示名
        /// </summary>
        [Description("字段显示名")]
        public string DisplayName { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [Description("单位")]
        public string Unit { get; set; }
        /// <summary>
        /// 是否单选
        /// </summary>
        [Description("是否单选")]
        public bool IsSingleSelect { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        [Description("是否启用")]
        public bool Enable { get; set; } = true;
        /// <summary>
        /// 执行方法
        /// </summary>
        [Description("执行方法")]
        public string ExecuteMethod { get; set; }
        /// <summary>
        /// 返回结果类型
        /// </summary>
        [Description("返回结果类型")]
        public string ResultType { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int Sort { get; set; }
    }

    public static class WorkFlowModuleSceneConfigInitDtoExtersion
    {
        public static InitFlowConfigRequestDO AddFieldConfig(this InitFlowConfigRequestDO moduleSceneConfigInitDto,
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
            moduleSceneConfigInitDto.FieldConfigs.Add(new InitFlowFieldConfigRequestDO
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
