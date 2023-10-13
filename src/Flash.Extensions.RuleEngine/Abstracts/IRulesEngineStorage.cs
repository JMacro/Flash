using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extensions.RuleEngine
{
    /// <summary>
    /// 规则引擎仓储
    /// </summary>
    public interface IRulesEngineStorage
    {
        /// <summary>
        /// 根据名称获得规则配置
        /// </summary>
        /// <param name="workflowName"></param>
        /// <returns></returns>
        ValueTask<WorkflowConfig> GetRuleInfo(string workflowName);
        /// <summary>
        /// 获得所有规则配置
        /// </summary>
        /// <returns></returns>
        ValueTask<List<WorkflowConfig>> GetAll();
        /// <summary>
        /// 新增规则配置
        /// </summary>
        /// <param name="workflowConfig"></param>
        /// <returns></returns>
        ValueTask<bool> AddRuleInfo(WorkflowConfig workflowConfig);
        /// <summary>
        /// 更新规则配置
        /// </summary>
        /// <param name="workflowConfig"></param>
        /// <returns></returns>
        ValueTask<bool> UpdateRuleInfo(WorkflowConfig workflowConfig);
        /// <summary>
        /// 删除规则配置
        /// </summary>
        /// <param name="workflowConfig"></param>
        /// <returns></returns>
        ValueTask<bool> RemoveRuleInfo(WorkflowConfig workflowConfig);
    }
}
