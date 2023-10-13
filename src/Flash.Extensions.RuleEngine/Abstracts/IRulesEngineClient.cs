using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extensions.RuleEngine
{
    /// <summary>
    /// 规则引擎客户端接口
    /// </summary>
    public interface IRulesEngineClient
    {
        /// <summary>
        /// 注册规则到引擎中
        /// </summary>
        /// <param name="jsonConfigs">规则配置（JSON格式），数据类型详见<see cref="WorkflowConfig"/></param>
        /// <returns></returns>
        IRulesEngineClient RegisterRules(string[] jsonConfigs);
        /// <summary>
        /// 注册规则到引擎中
        /// </summary>
        /// <param name="jsonConfig">规则配置（JSON格式），数据类型详见<see cref="WorkflowConfig"/></param>
        /// <returns></returns>
        IRulesEngineClient RegisterRules(string jsonConfig);
        /// <summary>
        /// 注册规则到引擎中
        /// </summary>
        /// <param name="workflowConfigs">规则配置</param>
        /// <returns></returns>
        IRulesEngineClient RegisterRules(IList<WorkflowConfig> workflowConfigs);
        /// <summary>
        /// 注册规则到引擎中
        /// </summary>
        /// <param name="workflowConfig">规则配置</param>
        /// <returns></returns>
        IRulesEngineClient RegisterRules(WorkflowConfig workflowConfig);
        /// <summary>
        /// 获得所有特性标记为 <see cref="RuleParameterAttribute"/> 的实例类并生成实体参数信息集合
        /// </summary>
        /// <param name="assemblies">程序集，若为Null则默认AppDomain.CurrentDomain.GetAssemblies</param>
        /// <returns></returns>
        List<EntityRuleParameterInfo> GetParameterInfosByAssemblys(params Assembly[] assemblies);
        /// <summary>
        /// 执行规则引擎
        /// </summary>
        /// <param name="workflowName"></param>
        /// <param name="inputs">Input datas</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        ValueTask<List<RuleResultTree>> ExecuteAsync(string workflowName, params object[] inputs);
        /// <summary>
        /// 规则配置编译
        /// </summary>
        /// <param name="workflowConfig">规则配置</param>
        /// <returns></returns>
        string BuilderRuleConfig(WorkflowConfig workflowConfig);
    }
}
