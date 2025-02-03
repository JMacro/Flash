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

using Flash.Extensions.Tracting.Skywalking.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Tracting.Skywalking
{
    public class InstrumentStartup : IInstrumentStartup
    {
        private readonly TracingDiagnosticProcessorObserver _observer;
        private readonly IEnumerable<IExecutionService> _services;
        private readonly ILogger _logger;
        private readonly IRuntimeEnvironment _runtimeEnvironment;
        public InstrumentStartup(TracingDiagnosticProcessorObserver observer, IEnumerable<IExecutionService> services, ILoggerFactory loggerFactory, IRuntimeEnvironment runtimeEnvironment)
        {
            _observer = observer;
            _services = services;
            _logger = loggerFactory.CreateLogger(typeof(InstrumentStartup));
            _runtimeEnvironment = runtimeEnvironment;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.LogInformation("Initializing ...");
            foreach (var service in _services)
                await service.StartAsync(cancellationToken);
            DiagnosticListener.AllListeners.Subscribe(_observer);
            _logger.LogInformation("Started SkyAPM .NET Core Agent.");
            // start init 
            _runtimeEnvironment.Initialized = true;
        }

        public async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var service in _services)
                await service.StopAsync(cancellationToken);
            _logger.LogInformation("Stopped SkyAPM .NET Core Agent.");
            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}
