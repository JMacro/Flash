using Flash.Extensions.Cache;
using Microsoft.AspNetCore.Mvc;
using System;
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
            return await this._cache.StringGetAsync<string>("11111111-1111-1111-1111-111111111111_Product_10d34c0d-ac63-4179-b22f-0031dc7d18c8");
        }

        [HttpGet("test2")]
        public bool Test2()
        {
            var result = this._distributedLock.Enter("", "", TimeSpan.FromSeconds(5));
            Thread.Sleep(50000);
            return result;
        }
    }
}
