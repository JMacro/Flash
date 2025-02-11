using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Flash.Extensions.RuleEngine
{
    /// <summary>
    /// 规则配置
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RuleConfig
    {
        /// <summary>
        /// Rule name for the Rule
        /// </summary>
        public string RuleName { get; set; }
        /// <summary>
        /// Rule description for the Rule
        /// </summary>
        public string RuleDescription { get; set; }
        /// <summary>	
        /// Gets or sets the custom property or tags of the rule.	
        /// </summary>	
        /// <value>	
        /// The properties of the rule.	
        /// </value>	
        public Dictionary<string, object> Properties { get; set; }
        /// <summary>
        /// Rule operator for the Rule
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RuleOperatorType? Operator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Gets or sets whether the rule is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        [JsonConverter(typeof(StringEnumConverter))]
        public RuleExpressionType RuleExpressionType { get; set; } = RuleExpressionType.LambdaExpression;
        public IEnumerable<string> WorkflowsToInject { get; set; }
        public IEnumerable<RuleConfig> Rules { get; set; }
        public IEnumerable<ScopedParam> LocalParams { get; set; }
        public string Expression { get; set; }
        public RuleActions Actions { get; set; }
        public string SuccessEvent { get; set; }
    }

    public enum RuleOperatorType
    {
        And = 0,
        AndAlso = 1,
        Or = 2,
        OrElse = 3
    }
}
