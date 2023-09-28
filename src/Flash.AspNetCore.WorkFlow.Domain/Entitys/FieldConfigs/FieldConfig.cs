using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs;
using Flash.AspNetCore.WorkFlow.Domain.Events;
using Flash.AspNetCore.WorkFlow.Infrastructure.Core;
using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.FieldConfigs
{
    public partial class FieldConfig : AggregateRoot
    {
        /// <summary>
        /// 工作流模块与场景配置Id
        /// <see cref="FlowConfigPO"/>
        /// </summary>
        [Description("工作流模块与场景配置Id")]
        public long WorkFlowModuleSceneConfigId { get; private set; }
        /// <summary>
        /// 表名
        /// </summary>
        [Description("表名")]
        public string TableName { get; private set; }
        /// <summary>
        /// 字段名
        /// </summary>
        [Description("字段名")]
        public string Name { get; private set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        [Description("数据类型")]
        public EWorkFlowFieldDataType Type { get; private set; }
        /// <summary>
        /// 字段显示名
        /// </summary>
        [Description("字段显示名")]
        public string DisplayName { get; private set; }
        /// <summary>
        /// 单位
        /// </summary>
        [Description("单位")]
        public string Unit { get; private set; }
        /// <summary>
        /// 是否单选
        /// </summary>
        [Description("是否单选")]
        public bool IsSingleSelect { get; private set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        [Description("是否启用")]
        public bool Enable { get; private set; }
        /// <summary>
        /// 执行方法
        /// </summary>
        [Description("执行方法")]
        public string ExecuteMethod { get; private set; }
        /// <summary>
        /// 返回结果类型
        /// </summary>
        [Description("返回结果类型")]
        public string ResultType { get; private set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int Sort { get; private set; }

        public long Id { get; private set; }
        public bool IsDelete { get; private set; }
        public DateTime CreateTime { get; private set; }
        public long CreateUserId { get; private set; }
        public DateTime LastModifyTime { get; private set; }
        public long LastModifyUserId { get; private set; }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="fieldConfigData"></param>
        public static FieldConfig Save(FieldConfigSaveData fieldConfigData)
        {
            var fieldConfig = new FieldConfig();
            var @event = new FieldConfigSaveEvent
            {
                Id = fieldConfigData.Id,
                AggregateId = fieldConfig.Id,
                WorkFlowModuleSceneConfigId = fieldConfigData.WorkFlowModuleSceneConfigId,
                Name = fieldConfigData.Name,
                TableName = fieldConfigData.TableName,
                Type = fieldConfigData.Type,
                DisplayName = fieldConfigData.DisplayName,
                Unit = fieldConfigData.Unit,
                IsSingleSelect = fieldConfigData.IsSingleSelect,
                Enable = fieldConfigData.Enable,
                ExecuteMethod = fieldConfigData.ExecuteMethod,
                ResultType = fieldConfigData.ResultType,
                Sort = fieldConfigData.Sort,
                Version = -1
            };
            fieldConfig.Apply(@event);
            return fieldConfig;
        }
    }
}
