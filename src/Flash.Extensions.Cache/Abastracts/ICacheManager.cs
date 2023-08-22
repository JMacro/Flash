using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flash.Extensions.Cache
{
    /// <summary>
    /// 缓存管理器
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// 缓存是否存在
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        bool KeyExists(string cacheKey);



        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        bool RemoveCache(string cacheKey);

        /// <summary>
        /// 设置缓存的过期时间
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheOutTime"></param>
        bool ExpireEntryAt(string cacheKey, TimeSpan cacheOutTime);

        #region String
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        T StringGet<T>(string cacheKey);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        Task<T> StringGetAsync<T>(string cacheKey);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        bool StringSet<T>(string cacheKey, T cacheValue);


        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        Task<bool> StringSetAsync<T>(string cacheKey, T cacheValue);

        /// <summary>
        /// 设置相对过期缓存，可以加缓存过期时间
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="cacheOutTime"></param>
        bool StringSet<T>(string cacheKey, T cacheValue, TimeSpan cacheOutTime);


        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="cacheOutTime"></param>
        Task<bool> StringSetAsync<T>(string cacheKey, T cacheValue, TimeSpan cacheOutTime);

        /// <summary>
        /// 数字递减
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        double StringDecrement(string cacheKey, double val = 1);

        /// <summary>
        /// 数字递增
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        double StringIncrement(string cacheKey, double val = 1);

        /// <summary>
        /// 数字递减
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        Task<double> StringDecrementAsync(string cacheKey, double val = 1);

        /// <summary>
        /// 数字递增
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        Task<double> StringIncrementAsync(string cacheKey, double val = 1);
        #endregion

        #region LOCK

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="lockValue"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        bool LockTake(string key, string lockValue, TimeSpan expiry);

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        string LockQuery(string key);

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="lockValue"></param>
        /// <returns></returns>
        bool LockRelease(string key, string lockValue);
        #endregion

        #region Publish&Subscrbe

        /// <summary>
        /// 发布一个事件
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        long Publish<T>(string channelId, T msg);

        /// <summary>
        /// 订阅一个事件
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        void Subscribe<T>(string channelId, Action<T> handler);

        /// <summary>
        /// 订阅一个事件
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        void Subscribe(string channelId, Action<object> handler);
        #endregion

        #region Hash
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        double HashIncrement(string cacheKey, string dataKey, double value = 1);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        double HashDecrement(string cacheKey, string dataKey, double value = 1);
        /// <summary>
        /// 获得指定的哈希key的哈希列表内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">指定的哈希key</param>
        /// <returns></returns>
        List<T> HashKeys<T>(string cacheKey);
        /// <summary>
        /// 获得指定的哈希key及数据key的内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">指定的哈希key</param>
        /// <param name="dataKey">数据Key（具有唯一性）</param>
        /// <returns></returns>
        T HashGet<T>(string cacheKey, string dataKey);
        /// <summary>
        /// 获得指定的哈希key的哈希列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">指定的哈希key</param>
        /// <returns></returns>
        IDictionary<string, T> HashGetAll<T>(string cacheKey);
        /// <summary>
        /// 设置哈希Item内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">指定的哈希key</param>
        /// <param name="dataKey">数据Key（具有唯一性）</param>
        /// <param name="value">数据内容</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        bool HashSet<T>(string cacheKey, string dataKey, T value, TimeSpan? expiry = default(TimeSpan?));
        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="keyValuePairs"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        void HashSet<T>(string cacheKey, IDictionary<string, T> keyValuePairs, TimeSpan? expiry = default(TimeSpan?));
        /// <summary>
        /// 删除指定的哈希数据
        /// </summary>
        /// <param name="cacheKey">指定的哈希key</param>
        /// <param name="dataKey">数据Key（具有唯一性）</param>
        /// <returns></returns>
        bool HashDelete(string cacheKey, params string[] dataKey);
        #endregion

        #region List
        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="dbNum"></param>
        /// <returns></returns>
        T ListLeftPop<T>(string cacheKey);

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="dbNum"></param>
        void ListLeftPush<T>(string cacheKey, T value);

        /// <summary>
        /// 获取列表长度
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="dbNum"></param>
        /// <returns></returns>
        long ListLength(string cacheKey);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="dbNum"></param>
        /// <returns></returns>
        List<T> ListRange<T>(string cacheKey);

        /// <summary>
        /// 移除一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="dbNum"></param>
        void ListRemove<T>(string cacheKey, T value);

        /// <summary>
        /// 入队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="dbNum"></param>
        void ListRightPush<T>(string cacheKey, T value);

        /// <summary>
        /// 出队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="dbNum"></param>
        T ListRightPush<T>(string cacheKey);

        /// <summary>
        /// 出队列后写入另外一个队列（原子操作）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="dbNum"></param>
        /// <returns></returns>
        T ListRightPopLeftPush<T>(string source, string destination);

        #endregion

        #region Set

        bool SetAdd<T>(string key, T value);

        bool SetContains<T>(string key, T value);

        long SetLength(string key);

        List<T> SetMembers<T>(string key);

        T SetPop<T>(string key);

        T SetRandomMember<T>(string key);

        List<T> SetRandomMembers<T>(string key, long count);

        bool SetRemove<T>(string key, T value);

        long SetRemove<T>(string key, T[] values);
        #endregion

        #region Execute
        /// <summary>
        /// 执行一段命令
        /// </summary>
        /// <param name="script"></param>
        /// <param name="objs"></param>
        /// <returns>RedisResult</returns>
        dynamic Execute(string script, params object[] objs);
        /// <summary>
        /// 执行一段命令
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="script"></param>
        /// <param name="objs"></param>
        /// <returns>泛型数据类型</returns>
        dynamic Execute<T>(string script, params object[] objs);
        /// <summary>
        /// 执行一段命令
        /// </summary>
        /// <param name="script"></param>
        /// <param name="objs"></param>
        /// <returns>RedisResult</returns>
        Task<dynamic> ExecuteAsync(string script, params object[] objs);
        /// <summary>
        /// 执行一段命令
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="script"></param>
        /// <param name="objs"></param>
        /// <returns>泛型数据类型</returns>
        Task<dynamic> ExecuteAsync<T>(string script, params object[] objs);
        /// <summary>
        /// 执行一段命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns>RedisResult</returns>
        dynamic ScriptEvaluate(string command, object parameters = null);
        /// <summary>
        /// 执行一段命令
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns>泛型数据类型</returns>
        dynamic ScriptEvaluate<T>(string command, object parameters = null);
        /// <summary>
        /// 执行一段命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns>RedisResult</returns>
        Task<dynamic> ScriptEvaluateAsync(string command, object parameters = null);
        /// <summary>
        /// 执行一段命令
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns>泛型数据类型</returns>
        Task<dynamic> ScriptEvaluateAsync<T>(string command, object parameters = null);
        #endregion

        #region 布隆过滤器
        /// <summary>
        /// 添加到布隆过滤器
        /// </summary>
        /// <param name="bloomFilterName">布隆过滤器名称</param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool BF4ADD(string bloomFilterName, string value);

        /// <summary>
        /// 添加到布隆过滤器
        /// </summary>
        /// <param name="bloomFilterName">布隆过滤器名称</param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> BF4ADDAsync(string bloomFilterName, string value);

        /// <summary>
        /// 是否存在布隆过滤器
        /// </summary>
        /// <param name="bloomFilterName">布隆过滤器名称</param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool BF4EXISTS(string bloomFilterName, string value);

        /// <summary>
        /// 是否存在布隆过滤器
        /// </summary>
        /// <param name="bloomFilterName">布隆过滤器名称</param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> BF4EXISTSAsync(string bloomFilterName, string value);
        #endregion

        /// <summary>
        /// 获得缓存Key名称
        /// </summary>
        /// <returns></returns>
        string GetCacheKey(string cacheKey);
    }
}
