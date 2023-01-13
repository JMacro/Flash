﻿using Flash.Extensions.Cache;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Test.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly ICacheManager _cache;
        private readonly IDistributedLock _distributedLock;

        public CacheController(Flash.Extensions.Cache.ICacheManager cache, IDistributedLock distributedLock)
        {
            this._cache = cache;
            this._distributedLock = distributedLock;
        }

        [HttpGet("test1")]
        public async Task<string> Test1()
        {
            await this._cache.StringSetAsync<string>("11111111-1111-1111-1111-111111111111_Product_10d34c0d-ac63-4179-b22f-0031dc7d18c8", "1");
            return await this._cache.StringGetAsync<string>("11111111-1111-1111-1111-111111111111_Product_10d34c0d-ac63-4179-b22f-0031dc7d18c8");
        }

        [HttpGet("test2")]
        public bool Test2()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            var result = this._distributedLock.Enter(key, value, TimeSpan.FromSeconds(60));
            Thread.Sleep(50000);
            this._distributedLock.Exit(key, value);
            return result;
        }

        [HttpGet("test3")]
        public string Test3()
        {
            var hashKey = "企业id_123123123";
            var dataKey = "业务参数/价格/推荐价比例";

            Random r1 = new Random(Guid.NewGuid().GetHashCode());
            Random r2 = new Random(Guid.NewGuid().GetHashCode());

            this._cache.HashSet(hashKey, dataKey, new { a = r1.Next(), b = r1.Next() });
            this._cache.HashSet(hashKey, "业务参数/价格/推荐价比例1", new { a = r2.Next(), b = r2.Next() });


            Dictionary<string, dynamic> keyValuePairs = new Dictionary<string, dynamic>();
            keyValuePairs.Add("dataKey", new { a = r1.Next(), b = r1.Next() });
            this._cache.HashSet(hashKey, keyValuePairs);


            return this._cache.HashGet<string>(hashKey, dataKey);
        }
    }
}