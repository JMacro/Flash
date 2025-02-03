using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Flash.Extensions.RuleEngine
{
    /// <summary>
    /// WorkflowConfig rules class for deserialization  the json config file
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class WorkflowConfig
    {
        /// <summary>
        /// Gets the workflow name.
        /// </summary>
        public string WorkflowName { get; set; }
        /// <summary>
        /// Gets the workflow description.
        /// </summary>
        public string WorkflowDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> WorkflowsToInject { get; set; }
        /// <summary>
        /// Gets or Sets the global params which will be applicable to all rules
        /// </summary>
        public IEnumerable<ScopedParam> GlobalParams { get; set; }
        /// <summary>
        /// list of rules.
        /// </summary>
        public IEnumerable<RuleConfig> Rules { get; set; }
    }
}
