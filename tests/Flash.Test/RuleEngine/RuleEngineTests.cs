using Flash.Extensions;
using Flash.Extensions.RuleEngine;
using Flash.Test.StartupTests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RulesEngine.Actions;
using RulesEngine.Extensions;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test.RuleEngine
{
    [TestFixture]
    public class RuleEngineTests : BaseTest<RulesEngineStartupTest>
    {
        private Stopwatch time = new Stopwatch();
        private IRulesEngineClient rulesEngine;

        private string Rule1 => this.ServiceProvider.GetService<IConfiguration>()["Rule1"];

        public override void Initialize()
        {
            base.Initialize();
            rulesEngine = this.ServiceProvider.GetService<IRulesEngineClient>();
        }

        [Test]
        public void BuliderRulesEngineTest()
        {
            Assert.IsNotNull(rulesEngine);
        }

        [Test]
        public void LoadConfigTest()
        {
            Assert.IsNotNull(rulesEngine);

            time.Restart();
            time.Start();
            rulesEngine.ExecuteAsync("Test1", new Buyer
            {
                Id = 666,
                Age = 16,
                Value = 2,
                Authenticated = false
            }).ConfigureAwait(false).GetAwaiter().GetResult();

            var buyer2 = new Buyer2
            {
                Age = 18,
                Value = 4,
            };
            rulesEngine.ExecuteAsync("Test2", new Buyer
            {
                Id = 666,
                Age = 18,
                Value = 2,
                Authenticated = true,
                buyer2 = buyer2
            }, buyer2).ConfigureAwait(false).GetAwaiter().GetResult();

            time.Stop();
            Console.WriteLine("规则引擎运行耗时:{0}", time.ElapsedMilliseconds);

            //resultList.OnSuccess((eventName) =>
            //{
            //    Console.WriteLine("成功事件触发:{0}", eventName);
            //});
        }

        [Test]
        public void GetParameterInfosTest()
        {
            Assert.IsNotNull(rulesEngine);

            time.Restart();
            time.Start();
            var result = rulesEngine.GetParameterInfosByAssemblys(AppDomain.CurrentDomain.GetAssemblies());
            time.Stop();
            Console.WriteLine("实体参数信息集合运行耗时:{0}", time.ElapsedMilliseconds);
            Console.WriteLine("结果:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        [Test]
        public void LocalFileStorageTest()
        {
            var engineStorage = this.ServiceProvider.GetService<IRulesEngineStorage>();
            Assert.IsNotNull(engineStorage);

            var workflow = engineStorage.GetRuleInfo("Test1").ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsNotNull(workflow);

            var workflows = engineStorage.GetAll().ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsNotNull(workflows);
        }
    }

    [RuleParameter("buyer")]
    public class Buyer
    {
        [RuleParameterInfo("Id")]
        public int Id { get; set; }
        [RuleParameterInfo("Age")]
        public int Age { get; set; }
        [RuleParameterInfo("Value")]
        public decimal Value { get; set; }
        /// <summary>
        /// 是否为已认证用户
        /// </summary>
        [RuleParameterInfo("Authenticated", "是否为已认证用户")]
        public bool Authenticated { get; set; }
        [RuleParameterInfo("Buyer2")]
        public Buyer2 buyer2 { get; set; }
        [RuleParameterInfo("Buyers2")]
        public List<Buyer2> buyers2 { get; set; }
    }

    [RuleParameter("buyer2")]
    public class Buyer2
    {
        [RuleParameterInfo("Id")]
        public int Id { get; set; }
        [RuleParameterInfo("Age")]
        public int Age { get; set; }
        [RuleParameterInfo("Value")]
        public decimal Value { get; set; }
        /// <summary>
        /// 是否为已认证用户
        /// </summary>
        [RuleParameterInfo("Authenticated", "是否为已认证用户")]
        public bool Authenticated { get; set; }
    }

    public class MyCustomAction : ActionBase
    {
        public override ValueTask<object> Run(ActionContext context, RuleParameter[] ruleParameters)
        {
            context.TryGetContext("Expression",out string output);

            if (context.GetParentRuleResult().Inputs.TryGetValue("buyer", out var value))
            {
                var buyer = (Buyer)value;
                buyer.Value = buyer.Value * 10;
            }
            return new ValueTask<object>(value);
        }
    }
}
