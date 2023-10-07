using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using Flash.AspNetCore.WorkFlow.Domain.Events;
using Flash.AspNetCore.WorkFlow.Infrastructure.Core;
using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using Flash.Core;
using Flash.Extensions;
using Flash.Extensions.ORM;
using MediatR;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.FieldConfigs
{
    public partial class FieldConfig : AggregateRoot, IEntity
    {
        private static object _Lock = new object();
        private static ConcurrentDictionary<string, IBaseRequest> CacheCommandHandleTypes = new ConcurrentDictionary<string, IBaseRequest>();

        /// <summary>
        /// 工作流模块与场景配置Id
        /// <see cref="FlowConfig"/>
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

        public static FieldConfig New()
        {
            return new FieldConfig();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="fieldConfigData"></param>
        public static FieldConfig Create(FieldConfigSaveData fieldConfigData)
        {
            var fieldConfig = new FieldConfig();
            var @event = new FieldConfigCreateEvent
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

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="fieldConfigData"></param>
        public static FieldConfig Update(FieldConfigSaveData fieldConfigData)
        {
            var fieldConfig = new FieldConfig();
            var @event = new FieldConfigUpdateEvent
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

        /// <summary>
        /// 检查字段值获取的执行方法是否异常
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public (bool Result, IBaseRequest BaseRequest) CheckExecuteMethodImplemented()
        {
            if (string.IsNullOrWhiteSpace(ExecuteMethod)) return (false, null);

            var executeCommandHandleType = EntityTypeCaches.TryGetOrAddByType(ExecuteMethod);
            if (executeCommandHandleType == null) throw new BusinessException($"无法找到类型{ExecuteMethod}");

            var commandHandleType = executeCommandHandleType.GetInterfaces().FirstOrDefault(p => p.HasImplementedRawGeneric(typeof(IRequestHandler<,>)));
            if (commandHandleType == null)
                throw new BusinessException($"{executeCommandHandleType.Name}未继承{typeof(IRequestHandler<,>).FullName}");

            var executeCommandType = commandHandleType.GetGenericArguments().First();
            lock (_Lock)
            {
                var result = CacheCommandHandleTypes.TryGetValue(ExecuteMethod, out IBaseRequest objCommandHandle);
                if (!result)
                {
                    if (Activator.CreateInstance(executeCommandType) is IBaseRequest baseRequest)
                    {
                        objCommandHandle = baseRequest;
                        CacheCommandHandleTypes.TryAdd(ExecuteMethod, objCommandHandle);
                        return (true, objCommandHandle);
                    }
                }
                return (result, objCommandHandle);
            }
        }
    }
}
