using Flash.AspNetCore.WorkFlow.Domain.Core;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowTypes;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeInstances;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeSettings;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeTypes;
using Flash.AspNetCore.WorkFlow.Domain.Events;
using Flash.AspNetCore.WorkFlow.Infrastructure.Core;
using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using Flash.Extensions.UidGenerator;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.WorkFlowTypes
{
    /// <summary>
    /// 工作流类型实体
    /// </summary>
    internal partial class FlowType : AggregateRoot        
    {
        public string BussinessName { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<INodeSetting> Settings { get; private set; }
        public IEnumerable<INodeType> Nodes { get; private set; }
        public IEnumerable<Behavior> Events { get; private set; }
        public IDictionary<string, string> AttachFormDatas { get; set; }
        public string FormTypeId { get; private set; }
        public bool IsDisable { get; private set; }
        public bool Completed { get; private set; }
        public string Description { get; private set; }
        public bool IsDraft { get; private set; }

        public long Id { get; private set; }
        public bool IsDelete { get; private set; }
        public DateTime CreateTime { get; private set; }
        public long CreateUserId { get; private set; }
        public DateTime LastModifyTime { get; private set; }
        public long LastModifyUserId { get; private set; }

        #region 业务检查
        /// <summary>
        /// 检查设置
        /// </summary>
        private bool CheckSettings(IEnumerable<INodeSetting> nodeSettings)
        {
            var result = true;
            if (nodeSettings != null)
            {
                foreach (var s in nodeSettings)
                {
                    if (s is ManualNodeSettings settings)
                    {
                        if (settings.ReminderEnable)
                        {
                            result = (settings.ReminderTimeSpan > 0 &&
                                !string.IsNullOrWhiteSpace(settings.ReminderNotifyUrl) &&
                                !string.IsNullOrEmpty(settings.ReminderNotifyMethod));
                        }

                        if (settings.EnableByAPI)
                        {
                            result = result && (!string.IsNullOrWhiteSpace(settings.ReminderCanInformApiMethod) &&
                                !string.IsNullOrWhiteSpace(settings.ReminderCanInformApiUrl));
                        }
                    }
                }
            }
            return result;
        }
        #endregion

        public void Init(WorkFlowTypeData workFlowTypeInitData)
        {
            var nodes = new Collection<INodeType>();
            var root = workFlowTypeInitData.Nodes.FirstOrDefault(node => node.IsRoot);

            GenerateNode(root, workFlowTypeInitData.Nodes, nodes, out var completed);
            var settingIsCompleted = CheckSettings(workFlowTypeInitData.Settings);

            var @event = new FlowTypeInitEvent
            {
                Id = this._uniqueIdGenerator.NewId(),
                AggregateId = this._uniqueIdGenerator.NewId(),
                AttachFormDatas = workFlowTypeInitData.AttachFormDatas,
                BussinessName = workFlowTypeInitData.BussinessName,
                Completed = completed && settingIsCompleted,
                Events = workFlowTypeInitData.Events,
                FormTypeId = workFlowTypeInitData.FormTypeId,
                IsDisable = false,
                Name = workFlowTypeInitData.Name,
                Nodes = workFlowTypeInitData.Nodes,
                Removed = false,
                Settings = workFlowTypeInitData.Settings,
                Description = workFlowTypeInitData.Description,
                IsDraft = true,
                Version = -1
            };
            Apply(@event);
        }
    }
}
