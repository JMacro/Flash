using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Commands.FlowForms.GetFlowFormDetial
{
    internal class GetFlowFormDetialQuery : IRequest<GetFlowFormDetialQueryResult>
    {
        public long Id { get; set; }
    }
}
