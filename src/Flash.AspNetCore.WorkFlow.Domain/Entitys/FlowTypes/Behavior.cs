using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowTypes
{
    public class Behavior
    {
        public BehaviorTriggersEnum Trigger { get; set; }
        public string Method { get; set; }
        public string Url { get; set; }
        public IDictionary<string, string> Header { get; set; }
        public string Body { get; set; }
        public string MediaType { get; set; }
        public string RuntimeUrl { get; set; }
        public IDictionary<string, string> RuntimeHeader { get; set; }
        public string RuntimeBody { get; set; }
        public string RuntimeResponseContent { get; set; }
        public int RuntimeResponseStatusCode { get; set; }
    }

    public enum BehaviorTriggersEnum
    {
        OnFlowInitedEvent = 0,
        OnFlowEndEvent = 1,
        OnFlowLaunchedEvent = 2,
        OnFlowSuspendedEvent = 3,
        OnFlowAbortedEvent = 4,
        OnFlowResumeEvent = 5,
        OnNodeBefroeHandledEvent = 6,
        OnNodeAfterHandledEvent = 7,
    }
}
