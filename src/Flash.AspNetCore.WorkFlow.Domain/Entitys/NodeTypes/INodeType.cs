using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowTypes;
using System.Collections.Generic;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeTypes
{
    /// <summary>
    /// 节点类型抽象
    /// </summary>
    public interface INodeType
    {
        bool IsRoot { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        IList<INodeType> SubNodes { get; set; }
        IDictionary<string, string> ParamData { get; set; }
        IDictionary<string, string> ResultData { get; set; }
        bool Display { get; set; }
        bool Preview { get; set; }
        string RepulseNodeId { get; set; }
        IEnumerable<Behavior> Events { get; set; }
    }

    /// <summary>
    /// 节点类型基类
    /// </summary>
    public abstract class NodeType : INodeType
    {
        public bool IsRoot { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<INodeType> SubNodes { get; set; } = new List<INodeType>();
        public IDictionary<string, string> ParamData { get; set; } = new Dictionary<string, string>();
        public IDictionary<string, string> ResultData { get; set; } = new Dictionary<string, string>();
        public bool Display { get; set; }
        public bool Preview { get; set; }
        public string RepulseNodeId { get; set; }
        public IEnumerable<Behavior> Events { get; set; }
    }
}
