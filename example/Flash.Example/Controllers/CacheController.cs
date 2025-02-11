using Flash.Extensions.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Flash.Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly ICacheManager _cacheManager;

        public CacheController(ICacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        [HttpGet("StringSet")]
        public async Task<bool> StringSet(string CacheKey, string CacheValue)
        {
            var result = _cacheManager.StringSet(CacheKey, CacheValue, TimeSpan.FromDays(1));
            result = await _cacheManager.StringSetAsync(CacheKey, CacheValue, TimeSpan.FromDays(1));
            return result;
        }
    }
}
