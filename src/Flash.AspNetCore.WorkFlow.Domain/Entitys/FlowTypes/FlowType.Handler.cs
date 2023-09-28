using Flash.AspNetCore.WorkFlow.Domain.Core;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeTypes;
using Flash.AspNetCore.WorkFlow.Domain.Events;
using MediatR;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.WorkFlowTypes
{
    internal partial class FlowType : IAggregateRootHandler<FlowTypeInitEvent>
    {
        public void Handle(FlowTypeInitEvent request)
        {
            var root = request.Nodes.FirstOrDefault(node => node.IsRoot);
            var nodes = new Collection<INodeType>();
            GenerateNode(root, request.Nodes, nodes, out var completed);

            Id = request.AggregateId;
            AttachFormDatas = request.AttachFormDatas;
            BussinessName = request.BussinessName;
            Completed = request.Completed;
            Events = request.Events;
            FormTypeId = request.FormTypeId;
            IsDisable = request.IsDisable;
            Name = request.Name;
            Nodes = nodes;
            Removed = request.Removed;
            Settings = request.Settings;
            Description = request.Description;
            IsDraft = request.IsDraft;
            Version = request.Version;
        }
    }
}
