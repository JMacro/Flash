using AutoMapper;
using Castle.Core.Logging;
using Flash.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Flash.Extensions.RuleEngine
{
    /// <summary>
    /// 规则引擎客户端
    /// </summary>
    public sealed class RulesEngineClient : IRulesEngineClient
    {
        private readonly IMapper _mapper;
        private readonly IRulesEngine _rulesEngine;
        private readonly IRulesEngineStorage _engineStorage;
        private readonly ILogger<RulesEngineClient> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="rulesEngine">规则引擎实例</param>
        /// <param name="engineStorage"></param>
        /// <param name="logger"></param>
        public RulesEngineClient(
            IMapper mapper,
            IRulesEngine rulesEngine,
            IRulesEngineStorage engineStorage,
            ILogger<RulesEngineClient> logger)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._rulesEngine = rulesEngine ?? throw new ArgumentNullException(nameof(rulesEngine));
            this._engineStorage = engineStorage;
            this._logger = logger;
        }

        #region 引擎规则注册
        /// <inheritdoc/>
        public IRulesEngineClient RegisterRules(string[] jsonConfigs)
        {
            if (jsonConfigs != null && jsonConfigs.Any())
            {
                var workflows = Enumerable.Select(jsonConfigs, (string item) => JsonConvert.DeserializeObject<Workflow>(item)).ToArray();
                this._rulesEngine.AddWorkflow(workflows);
            }
            return this;
        }

        /// <inheritdoc/>
        public IRulesEngineClient RegisterRules(string jsonConfig)
        {
            if (!string.IsNullOrWhiteSpace(jsonConfig))
            {
                var workflows = JsonConvert.DeserializeObject<List<Workflow>>(jsonConfig);
                if (workflows != null && workflows.Any())
                {
                    this._rulesEngine.AddWorkflow(workflows.ToArray());
                }
            }
            return this;
        }

        /// <inheritdoc/>
        public IRulesEngineClient RegisterRules(IList<WorkflowConfig> workflowConfigs)
        {
            if (workflowConfigs.Any())
            {
                var workflows = this._mapper.Map<List<Workflow>>(workflowConfigs);
                this._rulesEngine.AddWorkflow(workflows.ToArray());
            }
            return this;
        }

        /// <inheritdoc/>
        public IRulesEngineClient RegisterRules(WorkflowConfig workflowConfig)
        {
            if (workflowConfig != null)
            {
                return RegisterRules(new List<WorkflowConfig> { workflowConfig });
            }
            return this;
        }
        #endregion

        /// <inheritdoc/>
        public List<EntityRuleParameterInfo> GetParameterInfosByAssemblys(params Assembly[] assemblies)
        {
            var result = new List<EntityRuleParameterInfo>();
            assemblies = assemblies ?? AppDomain.CurrentDomain.GetAssemblies();

            var types = assemblies.SelectMany(a => a.GetTypes()).Where(p =>
            {
                return Attribute.GetCustomAttributes(p, true).Is<RuleParameterAttribute>();
            }).Select(p =>
            {
                (Type Type, RuleParameterAttribute RuleParameterAttribute) data = (p, (Attribute.GetCustomAttribute(p, typeof(RuleParameterAttribute), inherit: false) as RuleParameterAttribute));
                return data;
            }).ToList();

            foreach (var item in types)
            {
                var properties = EntityTypeCaches.TryGetOrAddByProperties(item.Type);
                var entityParameterInfo = new EntityRuleParameterInfo
                {
                    FullName = $"{item.Type.FullName}, {item.Type.Assembly.GetName().Name}",
                    EntityAlias = item.RuleParameterAttribute.Name,
                    Description = item.RuleParameterAttribute.Description,
                    Children = new List<RuleParameterInfo>()
                };
                BuilderEntityParameter(item, entityParameterInfo.Children);
                result.Add(entityParameterInfo);
            }
            return result;

            void BuilderEntityParameter((Type Type, RuleParameterAttribute RuleParameterAttribute) entityParameter, List<RuleParameterInfo> children, int recursion = 0)
            {
                var properties = EntityTypeCaches.TryGetOrAddByProperties(entityParameter.Type);
                var entityParameterInfo = new EntityRuleParameterInfo
                {
                    EntityAlias = entityParameter.RuleParameterAttribute.Name,
                    Description = entityParameter.RuleParameterAttribute.Description,
                    Children = children
                };

                foreach (var propertie in properties)
                {
                    var parameterInfoAttribute = propertie.GetCustomAttribute<RuleParameterInfoAttribute>();
                    if (parameterInfoAttribute != null)
                    {
                        var parameterInfo = new RuleParameterInfo
                        {
                            EntityAlias = entityParameter.RuleParameterAttribute.Name,
                            ParameterName = parameterInfoAttribute.Name,
                            ParameterType = MapperParameterType(propertie.PropertyType),
                            ParameterDescription = parameterInfoAttribute.Description
                        };
                        entityParameterInfo.Children.Add(parameterInfo);
                    }
                }

                string MapperParameterType(Type propertyType)
                {
                    if (typeof(System.Collections.IList).IsAssignableFrom(propertyType) ||
                        typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType))
                        return "Array";

                    if (typeof(System.Collections.IDictionary).IsAssignableFrom(propertyType))
                        return "Dictionary";

                    if (propertyType.IsClass) return "Object";
                    return propertyType.Name;
                }

            }
        }

        /// <inheritdoc/>
        public async ValueTask<List<RuleResultTree>> ExecuteAsync(string workflowName, params object[] inputs)
        {
            Check.Argument.IsNotEmpty(workflowName, nameof(workflowName));
            Check.Argument.IsNotNull(inputs, nameof(inputs), "输入实体不允许为空");

            //判断规则引擎中是否存在指定规则
            if (!this._rulesEngine.ContainsWorkflow(workflowName))
            {
                var workflowInfo = await this._engineStorage.GetRuleInfo(workflowName);
                Check.Argument.IsNotNull(workflowInfo, nameof(workflowName), $"{workflowName}规则不存在");

                RegisterRules(workflowInfo);
            }

            var list = new List<RuleParameter>();
            for (int i = 0; i < inputs.Length; i++)
            {
                list.Add(BuilderRuleParameter(inputs[i]));
            }
            var resultList = await this._rulesEngine.ExecuteAllRulesAsync(workflowName, list.ToArray());
            foreach (var item in resultList)
            {
                //item.ActionResult.Output
                this._logger.LogInformation("规则名称：{0},    验证结果：{1},    验证备注：{2}", item.Rule.RuleName, item.IsSuccess, item.ExceptionMessage);
            }
            return resultList;
        }

        /// <inheritdoc/>
        public string BuilderRuleConfig(WorkflowConfig workflowConfig)
        {
            return "";
        }

        #region 私有方法
        /// <summary>
        /// RuleParameter Builder
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private RuleParameter BuilderRuleParameter(object source)
        {
            var sourceType = source.GetType();
            var ruleParameterAttribute = TryGet(sourceType.CustomAttributes);
            if (ruleParameterAttribute == null) return default(RuleParameter);

            return new RuleParameter(ruleParameterAttribute.Name, source);
        }

        /// <summary>
        /// 尝试获得指定Attribute类型元素
        /// </summary>
        /// <param name="customAttributes"></param>
        /// <returns></returns>
        private RuleParameterAttribute TryGet(IEnumerable<CustomAttributeData> customAttributes)
        {
            var customAttribute = customAttributes.FirstOrDefault(p => AttributeExtensions.Is<RuleParameterAttribute>(p));
            if (customAttribute == null) return default(RuleParameterAttribute);
            var conStruct = typeof(RuleParameterAttribute).GetConstructor(new Type[] { typeof(string), typeof(string) });
            var result = (RuleParameterAttribute)conStruct.Invoke(customAttribute.ConstructorArguments.Select(p => p.Value).ToArray());
            return result;
        }
        #endregion
    }
}
