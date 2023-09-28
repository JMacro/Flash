using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowTypes;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeInstances
{
    /// <summary>
    /// 节点实例抽象
    /// </summary>
    public interface INodeInstance : IRequest
    {
        bool IsRoot { get; set; }
        FlowInstance FlowInstance { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        IList<INodeInstance> SubNodes { get; set; }
        INodeInstance Next { get; }
        NodeStatus Status { get; set; }
        DateTime HandledByUtc { get; set; }
        IDictionary<string, string> ParamData { get; set; }
        IDictionary<string, string> ResultData { get; set; }
        IDictionary<string, string> RuntimeParamData { get; set; }
        IDictionary<string, string> RuntimeResultData { get; set; }
        bool Display { get; set; }
        bool Preview { get; set; }
        string RepulseNodeId { get; set; }
        IEnumerable<Behavior> Events { get; set; }

    }

    public abstract class NodeInstance : INodeInstance
    {
        public bool IsRoot { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<INodeInstance> SubNodes { get; set; } = new List<INodeInstance>();
        [JsonIgnore]
        public virtual INodeInstance Next
        {
            get
            {
                if (SubNodes.Count == 0)
                    return null;
                else
                    return SubNodes[0];
            }
        }
        public NodeStatus Status { get; set; }
        public DateTime HandledByUtc { get; set; }
        public IDictionary<string, string> ParamData { get; set; }
        public IDictionary<string, string> ResultData { get; set; }
        public IDictionary<string, string> RuntimeParamData { get; set; }
        public IDictionary<string, string> RuntimeResultData { get; set; }
        public bool Display { get; set; }
        public FlowInstance FlowInstance { get; set; }
        public bool Preview { get; set; }
        public string RepulseNodeId { get; set; }
        public IEnumerable<Behavior> Events { get; set; }
    }

    public enum NodeStatus
    {
        /// <summary>
        /// 节点未被处理
        /// </summary>
        UnHandled = 0,
        /// <summary>
        /// 等待多个处理结果汇总（正在处理中）
        /// </summary>
        Watting = 1,
        /// <summary>
        /// 阻塞（流程暂停)
        /// </summary>
        Blocked = 2,
        /// <summary>
        /// 通过
        /// </summary>
        Passed = 3,
        /// <summary> 
        /// 不通过
        /// </summary>
        NotPassed = 4
    }
}
