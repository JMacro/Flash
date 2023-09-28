﻿using Flash.AspNetCore.WorkFlow.Domain.Entitys.FieldConfigs;
using Flash.AspNetCore.WorkFlow.Domain.Events;
using Flash.AspNetCore.WorkFlow.Infrastructure.Core;
using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using Flash.Extensions.ORM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Xml.Linq;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs
{
    /// <summary>
    /// 流程配置实体
    /// </summary>
    public partial class FlowConfig : AggregateRoot, IEntity
    {
        /// <summary>
        /// 父Id
        /// </summary>
        public long ParentId { get; private set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 当Type=1并SubType=2时为流程TypeId
        /// </summary>
        [Description("对接方目标Id")]
        [StringLength(64)]
        public long ObjectId { get; private set; }
        /// <summary>
        /// 流程配置类型
        /// </summary>
        [Description("流程配置类型")]
        public EWorkFlowConfigType Type { get; private set; }
        /// <summary>
        /// 流程配置类型
        /// </summary>
        [Description("流程配置子类型")]
        public EWorkFlowConfigSubType SubType { get; private set; }
        /// <summary>
        /// 分类类型
        /// </summary>
        [Description("分类类型")]
        public short ClassType { get; private set; }
        /// <summary>
        /// 分类子类型
        /// </summary>
        [Description("分类子类型")]
        public short ClassSubType { get; private set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; private set; }
        public long Id { get; private set; }
        public bool IsDelete { get; private set; }
        public DateTime CreateTime { get; private set; }
        public long CreateUserId { get; private set; }
        public DateTime LastModifyTime { get; private set; }
        public long LastModifyUserId { get; private set; }

        private List<FieldConfig> _fieldConfigs = new List<FieldConfig>();

        private FlowConfig() { }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="flowConfigData"></param>
        public static FlowConfig Create(FlowConfigCreateData flowConfigData, List<FieldConfigSaveData> fieldConfigDatas)
        {
            var flowConfig = new FlowConfig();
            foreach (var item in fieldConfigDatas)
            {
                var @event = new FieldConfigSaveEvent
                {
                    Id = flowConfigData.Id,
                    AggregateId = flowConfig.Id,
                    WorkFlowModuleSceneConfigId = item.WorkFlowModuleSceneConfigId,
                    Name = item.Name,
                    TableName = item.TableName,
                    Type = item.Type,
                    DisplayName = item.DisplayName,
                    Unit = item.Unit,
                    IsSingleSelect = item.IsSingleSelect,
                    Enable = item.Enable,
                    ExecuteMethod = item.ExecuteMethod,
                    ResultType = item.ResultType,
                    Sort = item.Sort,
                    Version = -1
                };
                flowConfig.Apply(@event);
            }
            return flowConfig;
        }

        ///// <summary>
        ///// 创建
        ///// </summary>
        ///// <param name="data"></param>
        //public static FlowConfig Update(FlowConfigCreateData data)
        //{
        //    var flowConfig = new FlowConfig();
        //    var @event = new FieldConfigSaveEvent
        //    {
        //        Id = data.Id,
        //        AggregateId = flowConfig.Id,
        //        ParentId = data.ParentId,
        //        Name = data.Name,
        //        ObjectId = data.ObjectId,
        //        Type = data.Type,
        //        SubType = data.SubType,
        //        ClassType = data.ClassType,
        //        ClassSubType = data.ClassSubType,
        //        Remark = data.Remark,
        //        Version = -1
        //    };
        //    flowConfig.Apply(@event);
        //    return flowConfig;
        //}
    }
}