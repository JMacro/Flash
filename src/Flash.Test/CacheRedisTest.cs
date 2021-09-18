using Autofac;
using Flash.Extensions.Cache;
using Flash.Extensions.Cache.Redis;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test
{
    [TestClass]
    public class CacheRedisTest : BaseTest
    {
        private readonly ICacheManager _cacheManager;
        private readonly IDistributedLock _distributedLock;

        public CacheRedisTest() : base()
        {
            this._cacheManager = this.container.Resolve<ICacheManager>();
            this._distributedLock = this.container.Resolve<IDistributedLock>();
        }

        [TestMethod]
        public void StringSet()
        {
            this._cacheManager.StringSet<int>("123", 123);
            var d = this._cacheManager.StringGet<int>("123");

            var ddd = this._distributedLock.Enter("122", "AAAAA", TimeSpan.FromSeconds(60));

        }

        [TestMethod]
        public void DistributedLock()
        {
            var ddd = this._distributedLock.Enter("122", "AAAAA", TimeSpan.FromSeconds(60));
        }

        [TestMethod]
        public async Task HealthCheck()
        {
            var webHostBuilder = new WebHostBuilder()
             .UseStartup<Startup>()
             .ConfigureServices(services =>
             {
                 services.AddFlash(setup =>
                 {
                     setup.AddCache(cache =>
                     {
                         cache.UseRedis(option =>
                         {
                             option.WithDb(10);
                             option.WithKeyPrefix("SystemClassName:TypeClassName");
                             option.WithPassword("tY7cRu9HG_jyDw2r");
                             option.WithReadServerList("192.168.109.237:63100");
                             option.WithWriteServerList("192.168.109.237:63100");
                             option.WithSsl(false);
                             option.WithDistributedLock(true);
                         });
                     });
                 });

                 //services.AddHealthChecks(check =>
                 //{
                 //    check.AddRedisCheck("192.168.109.237:63100", "192.168.109.237:63100,password=tY7cRu9HG_jyDw2r,allowAdmin=true,ssl=false,abortConnect=false,connectTimeout=5000");
                 //});
             })
             //.UseHealthChecks("/healthcheck")
             ;


            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest($"/healthcheck").GetAsync();

            var df = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public void BloomFilterTest()
        {
            var dfdf = Guid.NewGuid().ToString();

            var dfde = this._cacheManager.GetString4BloomFilter("111", TimeSpan.FromSeconds(60), () => dfdf);

            var dd = this._cacheManager.BF4ADD("À¬»øÓÊ¼þ²¼Â¡¹ýÂËÆ÷", dfdf);
            var ddd = this._cacheManager.BF4ADDAsync("À¬»øÓÊ¼þ²¼Â¡¹ýÂËÆ÷", "bbbb");

            var v1 = this._cacheManager.BF4EXISTS("À¬»øÓÊ¼þ²¼Â¡¹ýÂËÆ÷", "bbbb");

        }

        [TestMethod]
        public void TreeTest()
        {
            String key = "RoleInherit";
            RoleInherit tree = new RoleInherit();
            List<long> childIds = new List<long>();
            int max = 100;
            tree.ChildIds = childIds;
            for (int i = 0; i < max; i++)
            {
                tree.RoleId = i;
                tree.Name = ("½ÇÉ«" + i);
                childIds.Clear();
                if (i < (max - 1))
                {
                    childIds.Add(i + 1);
                }

                this._cacheManager.HashSet(key, i.ToString(), tree);
            }
        }

        [TestMethod]
        public void GetTreeTest()
        {
            StringBuilder lua = new StringBuilder();
            lua.AppendLine("local function getChild(currentnode, t, res)");
            lua.AppendLine("  if currentnode == nil or t == nil  then");
            lua.AppendLine("    return res");
            lua.AppendLine("  end");
            lua.AppendLine("  local nextNode = nil");
            lua.AppendLine("  local nextType = nil");
            lua.AppendLine("  if t == 'RoleId' and (type(currentnode) == 'number' or type(currentnode) == 'string') then");
            lua.AppendLine("    local treeNode = redis.call('HGET', @CacheKey, currentnode)");
            lua.AppendLine("    if treeNode then");
            lua.AppendLine("      local node = cjson.decode(treeNode)");
            lua.AppendLine("      table.insert(res, treeNode)");
            lua.AppendLine("      if node and node.ChildIds then");
            lua.AppendLine("        nextNode = node.ChildIds");
            lua.AppendLine("        nextType = 'ChildIds'");
            lua.AppendLine("      end");
            lua.AppendLine("    end");
            lua.AppendLine("  elseif t == 'ChildIds' then");
            lua.AppendLine("    nextNode = {}");
            lua.AppendLine("    nextType = 'ChildIds'");
            lua.AppendLine("    local treeNode  = nil");
            lua.AppendLine("    local node = nil");
            lua.AppendLine("    local cnt = 0");
            lua.AppendLine("    for _, val in ipairs(currentnode) do");
            lua.AppendLine("      treeNode = redis.call('HGET', @CacheKey, tostring(val))");
            lua.AppendLine("      if treeNode then");
            lua.AppendLine("        node = cjson.decode(treeNode)");
            lua.AppendLine("        table.insert(res, treeNode)");
            lua.AppendLine("        if node and node.ChildIds then");
            lua.AppendLine("          for _, val2 in ipairs(node.ChildIds) do");
            lua.AppendLine("            table.insert(nextNode, val2)");
            lua.AppendLine("            cnt = cnt + 1");
            lua.AppendLine("          end");
            lua.AppendLine("        end");
            lua.AppendLine("      end");
            lua.AppendLine("    end");
            lua.AppendLine("    if cnt == 0 then");
            lua.AppendLine("      nextNode = nil");
            lua.AppendLine("      nextType = nil");
            lua.AppendLine("    end");
            lua.AppendLine("  end");
            lua.AppendLine("  return getChild(nextNode, nextType, res)");
            lua.AppendLine("end");
            lua.AppendLine("if @CacheKey and @DataKey then");
            lua.AppendLine("  return getChild(@DataKey, 'RoleId', {})");
            lua.AppendLine("end");
            lua.AppendLine("return {}");

            var df = this._cacheManager.ScriptEvaluate<string>("return redis.call('GET',@key)", new { key = "DD" });
            var ddd = this._cacheManager.ScriptEvaluate<string>("return redis.call('SET',@key,@value)", new { key = "DD", value = 0 });

            var redisResult = this._cacheManager.ScriptEvaluate<RoleInherit>(lua.ToString(), new { CacheKey = "RoleInherit", DataKey = 0 });

            //var dd = redisResult.Type;

            //var redisValues = ((RedisValue[])redisResult);
            //var result = new List<RoleInherit>();
            //foreach (var item in redisValues)
            //{
            //    result.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<RoleInherit>(item.ToString()));
            //}

        }

    }

    public class RoleInherit
    {
        public long RoleId { get; set; }
        public string Name { get; set; }
        public List<long> ChildIds { get; set; }
    }
}
