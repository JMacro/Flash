using Microsoft.Extensions.DependencyInjection;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.RuleEngine
{
    public sealed class RulesEngineConfig
    {
        public ReSettings ReSettings { get; set; }
        public IRulesEngineStorage RulesEngineStorage { get; set; }
        public IServiceCollection Services { get; internal set; }
    }
}
