using Autofac.Core;
using Flash.Core;
using Flash.Extensions;
using Flash.Extensions.RuleEngine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using RulesEngine.Actions;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加规则引擎
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddRulesEngine(this IFlashHostBuilder builder, Action<RulesEngineConfig> action = null)
        {
            var rulesEngineConfig = new RulesEngineConfig();
            rulesEngineConfig.Services = builder.Services;
            if (action == null) action = new Action<RulesEngineConfig>((config) => { });
            action(rulesEngineConfig);
            Check.Argument.IsNotNull(rulesEngineConfig.RulesEngineStorage, nameof(rulesEngineConfig.RulesEngineStorage), $"请使用{nameof(UseLoaclFileStorage)}进行注册仓储或实现{nameof(IRulesEngineStorage)}接口并进行注册");
            builder.Services.TryAddScoped<IRulesEngineStorage>(sp => rulesEngineConfig.RulesEngineStorage);

            rulesEngineConfig.ReSettings = rulesEngineConfig.ReSettings ?? new ReSettings();
            rulesEngineConfig.ReSettings.CustomTypes = rulesEngineConfig.ReSettings.CustomTypes ?? new Type[] { };
            rulesEngineConfig.ReSettings.CustomActions = rulesEngineConfig.ReSettings.CustomActions ?? new Dictionary<string, Func<ActionBase>>();

            rulesEngineConfig.ReSettings.CustomTypes = BuilderCustomTypes(rulesEngineConfig.ReSettings.CustomTypes);
            rulesEngineConfig.ReSettings.CustomActions = BuilderCustomActions(rulesEngineConfig.ReSettings.CustomActions);

            builder.Services.TryAddSingleton<ReSettings>(rulesEngineConfig.ReSettings);
            builder.Services.TryAddSingleton<IRulesEngine>(sp =>
            {
                var rulesEngine = new RulesEngine.RulesEngine(rulesEngineConfig.ReSettings);
                return rulesEngine;
            });
            builder.Services.AddAutoMapper(config =>
            {
                config.CreateMap<WorkflowConfig, Workflow>();
                config.CreateMap<RuleConfig, Rule>();
            });
            builder.Services.TryAddScoped<IRulesEngineClient, RulesEngineClient>();
            builder.Services.AddHostedService<RegisterRulesHostedService>();
            return builder;
        }

        /// <summary>
        /// 使用本地文件作为仓储
        /// </summary>
        /// <param name="rulesEngineConfig"></param>
        /// <returns></returns>
        public static RulesEngineConfig UseLoaclFileStorage(this RulesEngineConfig rulesEngineConfig, IConfiguration configuration)
        {
            var workflowConfigs = configuration.GetSection("RulesEngine").Get<List<WorkflowConfig>>();
            rulesEngineConfig.RulesEngineStorage = new LocalFileRulesEngineStorage(workflowConfigs);
            return rulesEngineConfig;
        }

        private static Type[] BuilderCustomTypes(Type[] customTypes)
        {
            var customTypeDic = customTypes.ToDictionary(p => p.FullName);
            var selfCustomTypes = new Type[] {
                typeof(StringExtensions),
                typeof(UriExtensions),
                typeof(DecimalExtensions),
                typeof(BooleanExtensions),
                typeof(ObjectExtensions)
            };
            foreach (var customType in selfCustomTypes)
            {
                if (!customTypeDic.ContainsKey(customType.FullName))
                {
                    customTypeDic.Add(customType.FullName, customType);
                }
            }

            return customTypeDic.Values.ToArray();
        }

        private static Dictionary<string, Func<ActionBase>> BuilderCustomActions(Dictionary<string, Func<ActionBase>> customActions)
        {
            var result = new Dictionary<string, Func<ActionBase>>();
            var currentAssemblys = AppDomain.CurrentDomain.GetCurrentAssemblys("Microsoft", "System");
            var actionBases = currentAssemblys.SelectMany(p => p.GetTypes()).Where(p => typeof(ActionBase).IsAssignableFrom(p)).ToList();
            foreach (var actionBase in actionBases)
            {
                var obj = (ActionBase)Activator.CreateInstance(actionBase);
                result.Add(actionBase.Name, () => { return obj; });
            }
            return result;
        }
    }
}
