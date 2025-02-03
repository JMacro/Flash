﻿/*
 * Licensed to the SkyAPM under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The SkyAPM licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using Flash.Extensions.Tracting.Skywalking.Segments;
using System.Collections.Generic;

namespace Flash.Extensions.Tracting.Skywalking.Transport
{
    public class SegmentContextMapper : ISegmentContextMapper
    {
        public SegmentRequest Map(SegmentContext segmentContext)
        {
            var segmentRequest = new SegmentRequest
            {
                TraceId = segmentContext.TraceId,
                SegmentId = segmentContext.SegmentId,
                ServiceName = segmentContext.ServiceName,
                Identity = segmentContext.Identity
            };
            var span = new SpanRequest
            {
                SpanId = segmentContext.Span.SpanId,
                ParentSpanId = segmentContext.Span.ParentSpanId,
                OperationName = segmentContext.Span.OperationName,
                StartTime = segmentContext.Span.StartTime,
                EndTime = segmentContext.Span.EndTime,
                SpanType = (int)segmentContext.Span.SpanType,
                SpanLayer = (int)segmentContext.Span.SpanLayer,
                IsError = segmentContext.Span.IsError,
                Peer = segmentContext.Span.Peer,
                Component = segmentContext.Span.Component
            };
            foreach (var reference in segmentContext.References)
                span.References.Add(new SegmentReferenceRequest
                {
                    ParentSegmentId = reference.ParentSegmentId,
                    ParentServiceName = reference.ParentServiceName,
                    ParentSpanId = reference.ParentSpanId,
                    ParentEndpointName = reference.ParentEndpoint,
                    EntryServiceName = reference.EntryServiceName,
                    EntryEndpointName = reference.EntryEndpoint,
                    NetworkAddress = reference.NetworkAddress,
                    RefType = (int)reference.Reference
                });

            foreach (var tag in segmentContext.Span.Tags)
                span.Tags.Add(new KeyValuePair<string, string>(tag.Key, tag.Value));

            foreach (var log in segmentContext.Span.Logs)
            {
                var logData = new LogDataRequest { Timestamp = log.Timestamp };
                foreach (var data in log.Data)
                    logData.Data.Add(new KeyValuePair<string, string>(data.Key, data.Value));
                span.Logs.Add(logData);
            }

            segmentRequest.Spans.Add(span);
            return segmentRequest;
        }
    }
}
