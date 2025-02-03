using Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeInstances;
using System.Collections.Generic;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeTypes
{
    /// <summary>
    /// 人工节点类型
    /// </summary>
    public class ManualHandleNodeType : NodeType
    {
        public PriorityModEnum PriorityMod { get; set; }
        public string UserIdKey { get; set; }
        public IDictionary<string, string> Handlers { get; set; }
        public bool AllowReplaceHandler { get; set; }
        public string NotifyMethod { get; set; }
        public string NotifyURL { get; set; }
        public IDictionary<string, string> NotifyHeader { get; set; }
        public string NotifyBody { get; set; }
    }
}
