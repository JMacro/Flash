using Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeInstances;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowTypes
{
    public class WorkFlowTypeData
    {
        public string Id { get; set; }
        public string BussinessName { get; set; }
        public string Name { get; set; }
        public IEnumerable<INodeSetting> Settings { get; set; }
        public IEnumerable<NodeTypeInfoData> Nodes { get; set; }
        public IEnumerable<Behavior> Events { get; set; }
        public IDictionary<string, string> AttachFormDatas { get; set; }
        public string FormTypeId { get; set; }
        public string Description { get; set; }
    }

    public abstract class NodeTypeInfoData
    {
        public bool IsRoot { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<string> SubNodeId { get; set; } = new Collection<string>();
        public IDictionary<string, string> ParamData { get; set; }
        public IDictionary<string, string> ResultData { get; set; }
        public bool Display { get; set; }
        public bool Preview { get; set; }
        public string RepulseNodeId { get; set; }
        public IEnumerable<Behavior> Events { get; set; }
    }

    public class AutoHandleNodeTypeInfoData : NodeTypeInfoData
    {
        public bool Pass { get; set; }
    }
    public class ConditionNodeTypeInfoData : NodeTypeInfoData
    {
        public string Expression { get; set; }
    }

    public class HttpRequestNodeTypeInfoData : NodeTypeInfoData
    {
        public string Method { get; set; }
        public string URL { get; set; }
        public IDictionary<string, string> Header { get; set; }
        public string Body { get; set; }
        public int Timeout { get; set; }
        public int Retry { get; set; }
        public string MediaType { get; set; }
    }

    public class ManualHandleNodeTypeInfoData : NodeTypeInfoData
    {
        public PriorityModEnum PriorityMod { get; set; }
        public string UserIdKey { get; set; }
        public IDictionary<string, string> Handlers { get; set; }
        public ICollection<ManualHandleRequest> ManualHandleRequests { get; set; }
        public string ReminderId { get; set; }
        public bool AllowReplaceHandler { get; set; }
        public string NotifyMethod { get; set; }
        public string NotifyURL { get; set; }
        public IDictionary<string, string> NotifyHeader { get; set; }
        public string NotifyBody { get; set; }
        public string RuntimeNotifyURL { get; set; }
        public IDictionary<string, string> RuntimeNotifyHeader { get; set; }
        public string RuntimeNotifyBody { get; set; }
    }
}
