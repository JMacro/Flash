using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flash.Extensions.RuleEngine
{
    internal sealed class LocalFileRulesEngineStorage : IRulesEngineStorage
    {
        private readonly List<WorkflowConfig> _workflowConfigs;

        public LocalFileRulesEngineStorage(List<WorkflowConfig> workflowConfigs)
        {
            this._workflowConfigs = workflowConfigs;
        }

        /// <inheritdoc/>
        public ValueTask<WorkflowConfig> GetRuleInfo(string workflowName)
        {
            var result = this._workflowConfigs.FirstOrDefault(p => p.WorkflowName == workflowName);
            return new ValueTask<WorkflowConfig>(result);
        }

        /// <inheritdoc/>
        public ValueTask<List<WorkflowConfig>> GetAll()
        {
            return new ValueTask<List<WorkflowConfig>>(_workflowConfigs);
        }

        /// <inheritdoc/>
        public ValueTask<bool> AddRuleInfo(WorkflowConfig workflowConfig)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<bool> UpdateRuleInfo(WorkflowConfig workflowConfig)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<bool> RemoveRuleInfo(WorkflowConfig workflowConfig)
        {
            throw new NotImplementedException();
        }
    }
}
