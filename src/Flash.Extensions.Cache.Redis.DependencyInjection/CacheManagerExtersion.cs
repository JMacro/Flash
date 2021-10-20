using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Flash.Extensions.Cache.Redis
{
    public static class CacheManagerExtersion
    {
        /// <summary>
        /// 获得缓存数据（基于Redis的布隆过滤）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存key</param>
        /// <param name="cacheOutTime">缓存过期时间</param>
        /// <param name="func">待缓存数据的处理程序</param>
        /// <param name="bloomFilterName">布隆过滤器名称</param>
        /// <returns></returns>
        public static TResult GetString4BloomFilter<TResult>(this ICacheManager cache, string cacheKey, TimeSpan cacheOutTime, Func<TResult> func, string bloomFilterName = "DefaultBloomFilter")
        {
            var result = cache.StringGet<TResult>(cacheKey);
            if (result == null)
            {
                if (!cache.BF4EXISTS(bloomFilterName, cacheKey))
                {
                    return default(TResult);
                }
                else
                {
                    result = func();
                    cache.StringSet(cacheKey, result, CreateCacheOutTime(cacheOutTime));
                    cache.BF4ADD(bloomFilterName, cacheKey);
                }
            }
            return result;
        }

        /// <summary>
        /// 获得缓存数据（基于Redis的布隆过滤）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存key</param>
        /// <param name="cacheOutTime">缓存过期时间</param>
        /// <param name="func">待缓存数据的处理程序</param>
        /// <param name="bloomFilterName">布隆过滤器名称</param>
        /// <returns></returns>
        public static TResult GetString4BloomFilter<TResult>(this ICacheManager cache, string cacheKey, Func<TResult> func, TimeSpan cacheOutTime = default, string bloomFilterName = "DefaultBloomFilter")
        {
            var result = cache.StringGet<TResult>(cacheKey);
            if (result == null)
            {
                if (!cache.BF4EXISTS(bloomFilterName, cacheKey))
                {
                    return default(TResult);
                }
                else
                {
                    result = func();
                    cache.StringSet(cacheKey, result, CreateCacheOutTime(cacheOutTime));
                    cache.BF4ADD(bloomFilterName, cacheKey);
                }
            }
            return result;
        }

        /// <summary>
        /// 获得缓存数据（基于Redis的布隆过滤）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存key</param>
        /// <param name="cacheOutTime">缓存过期时间</param>
        /// <param name="func">待缓存数据的处理程序</param>
        /// <param name="bloomFilterName">布隆过滤器名称</param>
        /// <returns></returns>
        public static async Task<TResult> GetStringAsync4BloomFilter<TResult>(this ICacheManager cache, string cacheKey, TimeSpan cacheOutTime, Func<Task<TResult>> func, string bloomFilterName = "DefaultBloomFilter")
        {
            var result = await cache.StringGetAsync<TResult>(cacheKey);
            if (result == null)
            {
                if (!await cache.BF4EXISTSAsync(bloomFilterName, cacheKey))
                {
                    return default(TResult);
                }
                else
                {
                    result = await func();
                    await cache.StringSetAsync(cacheKey, result, CreateCacheOutTime(cacheOutTime));
                    await cache.BF4ADDAsync(bloomFilterName, cacheKey);
                }
            }
            return result;
        }

        /// <summary>
        /// 获得缓存数据（基于Redis的布隆过滤）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存key</param>
        /// <param name="cacheOutTime">缓存过期时间</param>
        /// <param name="func">待缓存数据的处理程序</param>
        /// <param name="bloomFilterName">布隆过滤器名称</param>
        /// <returns></returns>
        public static async Task<TResult> GetStringAsync4BloomFilter<TResult>(this ICacheManager cache, string cacheKey, Func<Task<TResult>> func, TimeSpan cacheOutTime = default, string bloomFilterName = "DefaultBloomFilter")
        {
            var result = await cache.StringGetAsync<TResult>(cacheKey);
            if (result == null)
            {
                if (!await cache.BF4EXISTSAsync(bloomFilterName, cacheKey))
                {
                    return default(TResult);
                }
                else
                {
                    result = await func();
                    await cache.StringSetAsync(cacheKey, result, CreateCacheOutTime(cacheOutTime));
                    await cache.BF4ADDAsync(bloomFilterName, cacheKey);
                }
            }
            return result;
        }

        /// <summary>
        /// 从缓存中获得类型为字符串的数据并序列化为指定对象
        /// </summary>
        /// <typeparam name="TResult">指定对象</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="func">待缓存数据的处理程序</param>
        /// <param name="cacheOutTime">缓存过期时间</param>
        /// <returns></returns>
        public static async Task<TResult> GetStringAsync<TResult>(this ICacheManager cache, string cacheKey, Func<Task<TResult>> func, TimeSpan cacheOutTime = default)
        {
            var result = await cache.StringGetAsync<TResult>(cacheKey);
            if (result == null)
            {
                result = await func();
                await cache.StringSetAsync(cacheKey, result, CreateCacheOutTime(cacheOutTime));
            }
            return result;
        }

        /// <summary>
        /// 从缓存中获得类型为字符串的数据并序列化为指定对象
        /// </summary>
        /// <typeparam name="TResult">指定对象</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="func">待缓存数据的处理程序</param>
        /// <param name="cacheOutTime">缓存过期时间</param>
        /// <returns></returns>
        public static TResult GetString<TResult>(this ICacheManager cache, string cacheKey, Func<TResult> func, TimeSpan cacheOutTime = default)
        {
            var result = cache.StringGet<TResult>(cacheKey);
            if (result == null)
            {
                result = func();
                cache.StringSet(cacheKey, result, CreateCacheOutTime(cacheOutTime));
            }
            return result;
        }

        /// <summary>
        /// 从缓存中获得类型为字符串的数据并序列化为指定对象
        /// </summary>
        /// <typeparam name="TResult">指定对象</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="dataKey">数据Key</param>
        /// <param name="func">待缓存数据的处理程序</param>
        /// <param name="cacheOutTime">缓存过期时间</param>
        /// <returns></returns>
        public static TResult GetHash<TResult>(this ICacheManager cache, string cacheKey, string dataKey, Func<TResult> func, TimeSpan cacheOutTime = default)
        {
            var result = cache.HashGet<TResult>(cacheKey, dataKey);
            if (result == null)
            {
                result = func();
                cache.HashSet(cacheKey, dataKey, result);
                cache.ExpireEntryAt(cacheKey, CreateCacheOutTime(cacheOutTime));
            }
            return result;
        }

        /// <summary>
        /// 从缓存中获得类型为字符串的数据并序列化为指定对象
        /// </summary>
        /// <typeparam name="TResult">指定对象</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="dataKeyFieldName">数据Key字段名称</param>
        /// <param name="func">待缓存数据的处理程序</param>
        /// <param name="cacheOutTime">缓存过期时间</param>
        /// <returns></returns>
        public static List<TResult> GetHashAll<TResult>(this ICacheManager cache, string cacheKey, string dataKeyFieldName, Func<List<TResult>> func, TimeSpan cacheOutTime = default)
        {
            if (string.IsNullOrWhiteSpace(dataKeyFieldName))
            {
                throw new ArgumentNullException("dataKeyFieldName不允许为空");
            }

            var type = typeof(TResult);
            var properties = type.GetProperties();
            var propertie = properties.FirstOrDefault(p => p.Name == dataKeyFieldName);
            if (propertie is null)
            {
                throw new ArgumentNullException($"{type.Name}不存在属性名{dataKeyFieldName}");
            }

            var cacheResult = cache.HashGetAll<TResult>(cacheKey);
            if (cacheResult == null || !cacheResult.Any())
            {
                var list = func();

                cacheResult = new Dictionary<string, TResult>();
                list.ForEach(item =>
                {
                    cacheResult.Add(propertie.GetValue(item).ToString(), item);
                });
                cache.HashSet(cacheKey, cacheResult);
                cache.ExpireEntryAt(cacheKey, CreateCacheOutTime(cacheOutTime));
            }
            return cacheResult.Values.ToList();
        }

        /// <summary>
        /// 从缓存中获得类型为字符串的数据并序列化为指定对象
        /// </summary>
        /// <typeparam name="TResult">指定对象</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="dataKeyFieldName">数据Key字段名称</param>
        /// <param name="func">待缓存数据的处理程序</param>
        /// <param name="cacheOutTime">缓存过期时间</param>
        /// <returns></returns>
        public static async Task<List<TResult>> GetHashAll<TResult>(this ICacheManager cache, string cacheKey, string dataKeyFieldName, Func<Task<List<TResult>>> func, TimeSpan cacheOutTime = default)
        {
            if (string.IsNullOrWhiteSpace(dataKeyFieldName))
            {
                throw new ArgumentNullException("dataKeyFieldName不允许为空");
            }

            var type = typeof(TResult);
            var properties = type.GetProperties().ToList();
            var propertie = properties.FirstOrDefault(p => p.Name == dataKeyFieldName);
            if (propertie is null)
            {
                throw new ArgumentNullException($"{type.Name}不存在属性名{dataKeyFieldName}");
            }

            var cacheResult = cache.HashGetAll<TResult>(cacheKey);
            if (cacheResult == null || !cacheResult.Any())
            {
                var list = await func();

                cacheResult = new Dictionary<string, TResult>();
                list.ForEach(item =>
                {
                    cacheResult.Add(propertie.GetValue(item).ToString(), item);
                });
                cache.HashSet(cacheKey, cacheResult);
                cache.ExpireEntryAt(cacheKey, CreateCacheOutTime(cacheOutTime));
            }
            return cacheResult.Values.ToList();
        }

        /// <summary>
        /// 创建过期时间
        /// </summary>
        /// <param name="cacheOutTime"></param>
        /// <returns></returns>
        private static TimeSpan CreateCacheOutTime(TimeSpan cacheOutTime)
        {
            if (cacheOutTime == default)
            {
                cacheOutTime = TimeSpan.FromHours(3);
            }

            var random = new Random(Guid.NewGuid().GetHashCode());
            return cacheOutTime.Add(TimeSpan.FromSeconds(random.Next(10, 30)));
        }
    }
}
