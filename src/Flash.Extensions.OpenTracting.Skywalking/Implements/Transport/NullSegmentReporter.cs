﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Tracting.Skywalking.Transport
{
    public class NullSegmentReporter : ISegmentReporter
    {
        public Task ReportAsync(IReadOnlyCollection<SegmentRequest> segmentRequests, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }
    }
}
