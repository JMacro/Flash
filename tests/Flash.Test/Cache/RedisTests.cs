using Autofac;
using Flash.Extensions.Cache;
using Flash.Extensions.Cache.Redis;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Flash.Extensions.HealthChecks;

namespace Flash.Test.Cache
{
    [TestFixture]
    public class RedisTests : BaseTest
    {
        private readonly ICacheManager _cacheManager;
        private readonly IDistributedLock _distributedLock;

        public RedisTests() : base()
        {

        }

        [Test]
        public void StringSetTest()
        {
            var cacheManager = container.Resolve<ICacheManager>();
            var result = cacheManager.StringSet("123", Guid.NewGuid(), TimeSpan.FromDays(1));
            Assert.IsTrue(result);
        }

        [Test]
        public void StringGetTest()
        {
            StringSetTest();

            var cacheManager = container.Resolve<ICacheManager>();
            var result = cacheManager.StringGet<string>("123");
            Assert.IsNotEmpty(result);
        }

        [Test]
        public void DistributedLockTest()
        {
            var distributedLock = container.Resolve<IDistributedLock>();
            var result = distributedLock.Enter($"{int.MaxValue}", Guid.NewGuid().ToString(), TimeSpan.FromSeconds(60));
            Assert.IsTrue(result);
        }

        [Test]
        public void HealthCheckTest()
        {
            var host = Environment.GetEnvironmentVariable("Redis_Host", EnvironmentVariableTarget.Machine);
            var password = Environment.GetEnvironmentVariable("Redis_Password", EnvironmentVariableTarget.Machine);

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
                             option.WithDb(0);
                             option.WithKeyPrefix("SystemClassName:TypeClassName");
                             option.WithPassword(password);
                             option.WithReadServerList(host);
                             option.WithWriteServerList(host);
                             option.WithSsl(false);
                             option.WithDistributedLock(true);
                         });
                     });
                 });

                 services.AddHealthChecks(check =>
                 {
                     check.AddRedisCheck($"{host}", $"{host},password={password},allowAdmin=true,ssl=false,abortConnect=false,connectTimeout=5000");
                 });
             })
             .UseHealthChecks("/healthcheck");

            var server = new TestServer(webHostBuilder);
            var response = server.CreateRequest($"/healthcheck").GetAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void BloomFilterTest()
        {
            var cacheManager = container.Resolve<ICacheManager>();

            var guid = Guid.NewGuid().ToString();

            var value1 = cacheManager.GetString4BloomFilter("111", TimeSpan.FromSeconds(60), () => guid);
            Assert.IsNotEmpty(value1);

            var result1 = cacheManager.BF4ADD("�����ʼ���¡������", guid);
            Assert.IsTrue(result1);

            var result2 = cacheManager.BF4ADDAsync("�����ʼ���¡������", Guid.NewGuid().ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(result2);

            var result3 = cacheManager.BF4EXISTS("�����ʼ���¡������", guid);
            Assert.IsTrue(result3);
        }


        [Test]
        public void GetTreeTest()
        {
            var cacheManager = container.Resolve<ICacheManager>();

            string key = "RoleInherit";
            RoleInherit tree = new RoleInherit();
            List<long> childIds = new List<long>();
            int max = 100;
            tree.ChildIds = childIds;
            for (int i = 0; i < max; i++)
            {
                tree.RoleId = i;
                tree.Name = "��ɫ" + i;
                childIds.Clear();
                if (i < max - 1)
                {
                    childIds.Add(i + 1);
                }

                cacheManager.HashSet(key, i.ToString(), tree, TimeSpan.FromDays(1));
            }

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

            var redisResult = cacheManager.ScriptEvaluate<RoleInherit>(lua.ToString(), new { CacheKey = cacheManager.GetCacheKey(key), DataKey = 98 });

            Assert.IsNotNull(redisResult);
        }

    }

    public class RoleInherit
    {
        public long RoleId { get; set; }
        public string Name { get; set; }
        public List<long> ChildIds { get; set; }
    }
}
