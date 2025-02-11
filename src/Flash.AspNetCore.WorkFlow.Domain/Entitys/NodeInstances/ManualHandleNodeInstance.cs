using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeInstances
{
    /// <summary>
    /// 人工节点实例
    /// </summary>
    public class ManualHandleNodeInstance : NodeInstance
    {
        public PriorityModEnum PriorityMod { get; set; }
        public string UserIdKey { get; set; }
        public IDictionary<string, string> Handlers { get; set; }
        public ICollection<ManualHandleRequest> ManualHandleRequests { get; set; }
        public bool AllowReplaceHandler { get; set; }
        public string NotifyMethod { get; set; }
        public string NotifyMediaType { get; set; }
        public string NotifyURL { get; set; }
        public IDictionary<string, string> NotifyHeader { get; set; }
        public string NotifyBody { get; set; }
        public string RuntimeNotifyURL { get; set; }
        public IDictionary<string, string> RuntimeNotifyHeader { get; set; }
        public string RuntimeNotifyBody { get; set; }

    }
    /// <summary>
    /// 多处理人通过方式
    /// </summary>
    public enum PriorityModEnum : byte
    {
        /// <summary>
        /// AND，会签，一票否决
        /// </summary>
        AndPriority = 0,
        /// <summary>
        /// OR，或签，一票通过
        /// </summary>
        OrPriority = 1,
        /// <summary>
        /// 第一时间处理优先
        /// </summary>
        FirstPriority = 2,
        /// <summary>
        /// 最后时间处理优先
        /// </summary>
        LatestPriority = 3,
        /// <summary>
        /// 超过50%处理结果为准,如果处理人为偶数，并且同意和不同意都一样，则默认为不通过
        /// </summary>
        MoreThanHalfPriority = 4,
        /// <summary>
        /// 加权处理结果优先，暂时没有加权策略，默认为不通过
        /// </summary>
        WeightedPriority = 5
    }
    /// <summary>
    /// 人工处理请求
    /// </summary>
    public class ManualHandleRequest
    {
        public string Id { get; set; }
        public string NodeId { get; set; }
        public string FlowInstanceId { get; set; }
        public string OriginHandlerId { get; set; }
        public string OriginHandlerType { get; set; }
        public string UserId { get; set; }
        public int Weight { get; set; }
        public HandleStatusEnum Status { get; set; }
        public DateTime HandledByUtc { get; set; }
        public IDictionary<string, string> Attachments { get; set; }
    }
    /// <summary>
    /// 处理人信息
    /// </summary>
    public class HandlerInfo
    {
        public string HandlerId { get; set; }
        public string HandlerType { get; set; }
        public string OriginHandlerId { get; set; }
        public string OriginHandlerType { get; set; }
        public int Weight { get; set; }
    }
    /// <summary>
    /// 处理状态
    /// </summary>
    public enum HandleStatusEnum
    {
        UnHandled = 0,
        Approval = 1,
        Reject = 2
    }
}
