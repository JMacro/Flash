using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.AspNetCore.WorkFlow.Domain.Commands.FlowForms.GetFlowFormDetial
{
    internal class GetFlowFormDetialQueryHandler : IRequestHandler<GetFlowFormDetialQuery, GetFlowFormDetialQueryResult>
    {
        public GetFlowFormDetialQueryHandler()
        {
        }

        public Task<GetFlowFormDetialQueryResult> Handle(GetFlowFormDetialQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
