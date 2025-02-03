using Flash.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flash.Extensions.RuleEngine
{
    /// <summary>
    /// 规则参数信息
    /// </summary>
    public sealed class EntityRuleParameterInfo
    {
        /// <summary>
        /// FullName
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 实体别名
        /// </summary>
        public string EntityAlias { get; set; }
        /// <summary>
        /// 参数描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 子参数集合
        /// </summary>
        public List<RuleParameterInfo> Children { get; set; }
    }

    /// <summary>
    /// 规则参数信息
    /// </summary>
    public sealed class RuleParameterInfo
    {
        /// <summary>
        /// 实体别名
        /// </summary>
        public string EntityAlias { get; set; }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// 参数类型
        /// </summary>
        public string ParameterType { get; set; }
        /// <summary>
        /// 参数描述
        /// </summary>
        public string ParameterDescription { get; set; }
        /// <summary>
        /// 全部名称
        /// </summary>
        public string FullName => $"{EntityAlias}.{ParameterName}";
    }

    /// <summary>
    /// 参数信息特性
    /// <para>用于批量获取系统所有标记该特性的</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = true)]
    public sealed class RuleParameterInfoAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public RuleParameterInfoAttribute(string name, string description = null)
        {
            Name = name;
            Description = description ?? name;
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; }
    }

    /// <summary>
    /// 规则实体别名Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RuleParameterAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">别名</param>
        /// <param name="description"></param>
        public RuleParameterAttribute(string name, string description = null)
        {
            Name = name;
            Description = description ?? name;
        }

        /// <summary>
        /// 别名
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; }
    }
}
