using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowTypes;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeTypes;
using Flash.Extensions.UidGenerator;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.WorkFlowTypes
{
    internal partial class FlowType
    {
        /// <summary>
        /// 生成自动处理节点
        /// </summary>
        public static INodeType CreateNode(AutoHandleNodeTypeInfoData nodeType)
        {
            var node = new AutoHandleNodeType
            {
                IsRoot = nodeType.IsRoot,
                Id = MicrosoftContainer.Instance.GetService<IUniqueIdGenerator>().NewId().ToString(),
                Name = nodeType.Name,
                ParamData = nodeType.ParamData,
                ResultData = nodeType.ResultData,
                Display = nodeType.Display,
                Pass = nodeType.Pass,
                Preview = nodeType.Preview,
                RepulseNodeId = nodeType.RepulseNodeId,
                Events = nodeType.Events,
            };

            return node;
        }
        /// <summary>
        /// 生成条件节点
        /// </summary>
        public static INodeType CreateNode(ConditionNodeTypeInfoData nodeType)
        {

            var node = new ConditionNodeType
            {
                IsRoot = nodeType.IsRoot,
                Expression = nodeType.Expression,
                Id = MicrosoftContainer.Instance.GetService<IUniqueIdGenerator>().NewId().ToString(),
                Name = nodeType.Name,
                ParamData = nodeType.ParamData,
                ResultData = nodeType.ResultData,
                Display = nodeType.Display,
                Preview = nodeType.Preview,
                RepulseNodeId = nodeType.RepulseNodeId,
                Events = nodeType.Events,
            };

            return node;
        }
        /// <summary>
        /// 生成http请求节点
        /// </summary>
        public static INodeType CreateNode(HttpRequestNodeTypeInfoData nodeType)
        {
            var node = new HttpRequestNodeType
            {
                IsRoot = nodeType.IsRoot,
                Id = MicrosoftContainer.Instance.GetService<IUniqueIdGenerator>().NewId().ToString(),
                Name = nodeType.Name,
                ParamData = nodeType.ParamData,
                ResultData = nodeType.ResultData,
                Display = nodeType.Display,
                Body = nodeType.Body,
                Header = nodeType.Header,
                Method = nodeType.Method,
                Retry = nodeType.Retry,
                Timeout = nodeType.Timeout,
                URL = nodeType.URL,
                MediaType = nodeType.MediaType,
                Preview = nodeType.Preview,
                RepulseNodeId = nodeType.RepulseNodeId,
                Events = nodeType.Events,
            };

            return node;
        }
        /// <summary>
        /// 生成人工处理节点
        /// </summary>
        public static INodeType CreateNode(ManualHandleNodeTypeInfoData nodeType)
        {
            var node = new ManualHandleNodeType
            {
                IsRoot = nodeType.IsRoot,
                Id = MicrosoftContainer.Instance.GetService<IUniqueIdGenerator>().NewId().ToString(),
                Name = nodeType.Name,
                ParamData = nodeType.ParamData,
                ResultData = nodeType.ResultData,
                Display = nodeType.Display,
                AllowReplaceHandler = nodeType.AllowReplaceHandler,
                Handlers = nodeType.Handlers,
                PriorityMod = nodeType.PriorityMod,
                UserIdKey = nodeType.UserIdKey,
                NotifyURL = nodeType.NotifyURL,
                NotifyHeader = nodeType.NotifyHeader,
                NotifyMethod = nodeType.NotifyMethod,
                NotifyBody = nodeType.NotifyBody,
                Preview = nodeType.Preview,
                RepulseNodeId = nodeType.RepulseNodeId,
                Events = nodeType.Events,
            };

            return node;
        }
        /// <summary>
        /// 生成自动处理节点
        /// </summary>
        public static NodeTypeInfoData CreateNode(AutoHandleNodeType nodeType)
        {
            var node = new AutoHandleNodeTypeInfoData
            {
                IsRoot = nodeType.IsRoot,
                Id = MicrosoftContainer.Instance.GetService<IUniqueIdGenerator>().NewId().ToString(),
                Name = nodeType.Name,
                ParamData = nodeType.ParamData,
                ResultData = nodeType.ResultData,
                Display = nodeType.Display,
                Pass = nodeType.Pass,
                Preview = nodeType.Preview,
                RepulseNodeId = nodeType.RepulseNodeId,
                Events = nodeType.Events,
            };

            return node;
        }
        /// <summary>
        /// 生成条件节点
        /// </summary>
        public static NodeTypeInfoData CreateNode(ConditionNodeType nodeType)
        {

            var node = new ConditionNodeTypeInfoData
            {
                IsRoot = nodeType.IsRoot,
                Expression = nodeType.Expression,
                Id = MicrosoftContainer.Instance.GetService<IUniqueIdGenerator>().NewId().ToString(),
                Name = nodeType.Name,
                ParamData = nodeType.ParamData,
                ResultData = nodeType.ResultData,
                Display = nodeType.Display,
                Preview = nodeType.Preview,
                RepulseNodeId = nodeType.RepulseNodeId,
                Events = nodeType.Events,
            };

            return node;
        }
        /// <summary>
        /// 生成http请求节点
        /// </summary>
        public static NodeTypeInfoData CreateNode(HttpRequestNodeType nodeType)
        {
            var node = new HttpRequestNodeTypeInfoData
            {
                IsRoot = nodeType.IsRoot,
                Id = MicrosoftContainer.Instance.GetService<IUniqueIdGenerator>().NewId().ToString(),
                Name = nodeType.Name,
                ParamData = nodeType.ParamData,
                ResultData = nodeType.ResultData,
                Display = nodeType.Display,
                Body = nodeType.Body,
                Header = nodeType.Header,
                Method = nodeType.Method,
                Retry = nodeType.Retry,
                Timeout = nodeType.Timeout,
                URL = nodeType.URL,
                MediaType = nodeType.MediaType,
                Preview = nodeType.Preview,
                RepulseNodeId = nodeType.RepulseNodeId,
                Events = nodeType.Events,
            };

            return node;
        }
        /// <summary>
        /// 生成人工处理节点
        /// </summary>
        public static NodeTypeInfoData CreateNode(ManualHandleNodeType nodeType)
        {
            var node = new ManualHandleNodeTypeInfoData
            {
                IsRoot = nodeType.IsRoot,
                Id = MicrosoftContainer.Instance.GetService<IUniqueIdGenerator>().NewId().ToString(),
                Name = nodeType.Name,
                ParamData = nodeType.ParamData,
                ResultData = nodeType.ResultData,
                Display = nodeType.Display,
                AllowReplaceHandler = nodeType.AllowReplaceHandler,
                Handlers = nodeType.Handlers,
                PriorityMod = nodeType.PriorityMod,
                UserIdKey = nodeType.UserIdKey,
                NotifyURL = nodeType.NotifyURL,
                NotifyHeader = nodeType.NotifyHeader,
                NotifyMethod = nodeType.NotifyMethod,
                NotifyBody = nodeType.NotifyBody,
                Preview = nodeType.Preview,
                RepulseNodeId = nodeType.RepulseNodeId,
                Events = nodeType.Events,
            };

            return node;
        }

        public static INodeType GenerateNode(NodeTypeInfoData nodeType, IEnumerable<NodeTypeInfoData> nodes, ICollection<INodeType> nodeTypes, out bool completed, IDictionary<string, string> repulseNodeIds = null)
        {
            completed = true;

            if (nodeType == null)
                return null;

            if (repulseNodeIds == null)
                repulseNodeIds = new Dictionary<string, string>();

            if (nodeType is ConditionNodeTypeInfoData && nodeType.SubNodeId.Count() != 2)
                completed = false;

            INodeType current = FlowType.CreateNode((dynamic)nodeType);
            repulseNodeIds.Add(nodeType.Id, current.Id);
            if (!string.IsNullOrWhiteSpace(current.RepulseNodeId) && repulseNodeIds.TryGetValue(current.RepulseNodeId, out var id))
                current.RepulseNodeId = id;

            var subNodes = nodeType.SubNodeId.Join(nodes, left => left, right => right.Id, (left, right) => right);

            foreach (var next in subNodes)
            {
                var subNode = GenerateNode(next, nodes, nodeTypes, out var subCompleted, repulseNodeIds);

                if (!completed || !subCompleted)
                    completed = false;

                current.SubNodes.Add(subNode);
            }

            nodeTypes.Add(current);
            return current;
        }
    }
}
