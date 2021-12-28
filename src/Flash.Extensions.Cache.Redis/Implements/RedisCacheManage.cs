﻿using Flash.Extensions.Tracting;
using Flash.LoadBalancer;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Cache.Redis
{
    public class RedisCacheManage : ICacheManager
    {
        #region 全局变量
        private static object _syncCreateInstance = new Object();

        private static object _syncCreateClient = new object();

        private static bool _supportSentinal = false;

        private static string _KeyPrefix = "";

        /// <summary>
        /// 虚拟节点数量
        /// </summary>
        private static readonly int _VIRTUAL_NODE_COUNT = 1024;

        /// <summary>
        /// Redis集群分片存储定位器
        /// </summary>
        private static Helpers.KetamaHash.KetamaNodeLocator _Locator;

        private static Dictionary<string, ConfigurationOptions> _clusterConfigOptions = new Dictionary<string, ConfigurationOptions>();

        private static Dictionary<string, Dictionary<int, ILoadBalancer<ClientHelper>>> _nodeClients = new Dictionary<string, Dictionary<int, ILoadBalancer<ClientHelper>>>();
        #endregion

        #region 实例变量
        private readonly int _DbNum = 0;
        /// <summary>
        /// 缓存连接数
        /// </summary>
        private readonly int _NumberOfConnections = 10;
        private readonly ITracerFactory _tracerFactory;

        #endregion

        private RedisCacheManage(int DbNum = 0, int NumberOfConnections = 10, ITracerFactory tracerFactory = null)
        {
            this._DbNum = DbNum;
            this._NumberOfConnections = NumberOfConnections;
            this._tracerFactory = tracerFactory;
        }

        /// <summary>
        /// 创建链接池管理对象
        /// </summary>
        public static ICacheManager Create(RedisCacheConfig config, ITracerFactory tracerFactory)
        {
            ThreadPool.SetMinThreads(200, 200);

            if (string.IsNullOrEmpty(config.KeyPrefix))
            {
                _KeyPrefix = string.Empty;
            }
            else
            {
                _KeyPrefix = config.KeyPrefix + ":";

            }
            if (_Locator == null)
            {
                lock (_syncCreateInstance)
                {
                    if (_Locator == null)
                    {
                        if (string.IsNullOrEmpty(config.SentineList) || !_supportSentinal)
                        {
                            //Redis服务器相关配置
                            string writeServerList = config.WriteServerList;
                            string readServerList = config.ReadServerList;
                            var writeServerArray = CacheConfigHelper.SplitString(writeServerList, ",").ToList();
                            var readServerArray = CacheConfigHelper.SplitString(readServerList, ",").ToList();
                            var Nodes = new List<string>();

                            //只有一个写,多个读的情况
                            /*
                             * Redis.ReadServerList	    192.168.100.51:16378,192.168.100.51:26378
                               Redis.WriteServerList	192.168.100.51:6378
                             */
                            if (writeServerArray.Count == 1)
                            {
                                var writeServer = writeServerArray[0];
                                var NodeName = writeServerArray[0];

                                if (!_clusterConfigOptions.ContainsKey(NodeName))
                                {
                                    ConfigurationOptions configOption = new ConfigurationOptions();
                                    configOption.ServiceName = NodeName;
                                    configOption.Password = config.Password;
                                    configOption.AbortOnConnectFail = false;
                                    configOption.DefaultDatabase = config.DBNum;
                                    configOption.Ssl = config.Ssl;

                                    foreach (var ipAndPort in writeServerArray.Union(readServerArray))
                                    {
                                        configOption.EndPoints.Add(ipAndPort);
                                    }

                                    _clusterConfigOptions.Add(writeServer, configOption);
                                }

                                Nodes.Add(NodeName);
                            }
                            /*
                             * 多个写和多个读
                              Redis.ReadServerList	    master-6378@192.168.100.51:16378,master-6379@192.168.100.51:16379,master-6380@192.168.100.51:16380,master-6381@192.168.100.51:16381,master-6382@192.168.100.51:16382,master-6378@192.168.100.51:26378,master-6379@192.168.100.51:26379,master-6380@192.168.100.51:26380,master-6381@192.168.100.51:26381,master-6382@192.168.100.51:26382
                              Redis.WriteServerList	    master-6378@192.168.100.51:6378,master-6379@192.168.100.51:6379,master-6380@192.168.100.51:6380,master-6381@192.168.100.51:6381,master-6382@192.168.100.51:6382         
                            */
                            else
                            {
                                for (int i = 0; i < writeServerArray.Count; i++)
                                {
                                    //存在多个Master服务器的时候
                                    if (writeServerArray[i].IndexOf("@") > 0)
                                    {
                                        //集群名称()
                                        var NodeName = CacheConfigHelper.GetServerClusterName(writeServerArray[i]);
                                        //主服务器名称
                                        var masterServer = CacheConfigHelper.GetServerHost(writeServerArray[i]);

                                        //主服务器列表
                                        var masterServerIPAndPortArray = CacheConfigHelper.GetServerList(config.WriteServerList, NodeName);
                                        //从服务器列表
                                        var slaveServerIPAndPortArray = CacheConfigHelper.GetServerList(config.ReadServerList, NodeName);

                                        //当前集群的配置不存在
                                        if (!_clusterConfigOptions.ContainsKey(NodeName))
                                        {
                                            ConfigurationOptions configOption = new ConfigurationOptions();
                                            configOption.ServiceName = NodeName;
                                            configOption.Password = config.Password;
                                            configOption.AbortOnConnectFail = false;
                                            configOption.DefaultDatabase = config.DBNum;
                                            configOption.Ssl = config.Ssl;
                                            configOption.ConnectTimeout = 15000;
                                            configOption.SyncTimeout = 5000;
                                            configOption.ResponseTimeout = 15000;

                                            foreach (var ipAndPort in masterServerIPAndPortArray.Union(slaveServerIPAndPortArray).Distinct())
                                            {
                                                configOption.EndPoints.Add(CacheConfigHelper.GetIP(ipAndPort), CacheConfigHelper.GetPort(ipAndPort));
                                            }

                                            _clusterConfigOptions.Add(NodeName, configOption);
                                        }

                                        Nodes.Add(NodeName);
                                    }
                                    else
                                    {
                                        //192.168.10.100:6379
                                        var NodeName = writeServerArray[i];

                                        if (!_clusterConfigOptions.ContainsKey(NodeName))
                                        {

                                            ConfigurationOptions configOption = new ConfigurationOptions();
                                            configOption.ServiceName = NodeName;
                                            configOption.Password = config.Password;
                                            configOption.AbortOnConnectFail = false;
                                            configOption.DefaultDatabase = config.DBNum;
                                            configOption.Ssl = config.Ssl;
                                            configOption.ConnectTimeout = 15000;
                                            configOption.SyncTimeout = 5000;
                                            configOption.ResponseTimeout = 15000;


                                            configOption.EndPoints.Add(CacheConfigHelper.GetIP(NodeName), CacheConfigHelper.GetPort(NodeName));
                                            _clusterConfigOptions.Add(NodeName, configOption);
                                        }

                                        Nodes.Add(NodeName);
                                    }
                                }
                            }

                            _Locator = new Helpers.KetamaHash.KetamaNodeLocator(Nodes, _VIRTUAL_NODE_COUNT);
                        }
                        else
                        {
                            List<string> sentinelMasterNameList = new List<string>();
                            List<string> sentinelServerHostList = new List<string>();
                            var SentineList = CacheConfigHelper.SplitString(config.SentineList, ",").ToList();
                            for (int i = 0; i < SentineList.Count; i++)
                            {
                                var args = CacheConfigHelper.SplitString(SentineList[i], "@").ToList();

                                var ServiceName = args[0];
                                var hostName = args[1];
                                var endPoint = CacheConfigHelper.SplitString(hostName, ":").ToList();
                                var ip = endPoint[0]; //IP
                                var port = int.Parse(endPoint[1]); //端口 

                                sentinelMasterNameList.Add(ServiceName);
                                sentinelServerHostList.Add(hostName);
                                if (!_clusterConfigOptions.ContainsKey(hostName))
                                {
                                    //连接sentinel服务器
                                    ConfigurationOptions sentinelConfig = new ConfigurationOptions();
                                    sentinelConfig.ServiceName = ServiceName;
                                    sentinelConfig.EndPoints.Add(ip, port);
                                    sentinelConfig.AbortOnConnectFail = false;
                                    sentinelConfig.DefaultDatabase = config.DBNum;
                                    sentinelConfig.TieBreaker = ""; //这行在sentinel模式必须加上
                                    sentinelConfig.CommandMap = CommandMap.Sentinel;
                                    sentinelConfig.DefaultVersion = new Version(3, 0);
                                    _clusterConfigOptions[hostName] = sentinelConfig;
                                }
                                else
                                {
                                    ConfigurationOptions sentinelConfig = _clusterConfigOptions[hostName] as ConfigurationOptions;
                                    sentinelConfig.EndPoints.Add(ip, port);
                                    _clusterConfigOptions[hostName] = sentinelConfig;
                                }
                            }

                            //初始化Reds分片定位器
                            _Locator = new Helpers.KetamaHash.KetamaNodeLocator(sentinelServerHostList, _VIRTUAL_NODE_COUNT);
                        }
                    }
                }
            }


            return new RedisCacheManage(config.DBNum, config.NumberOfConnections, tracerFactory);
        }

        #region 辅助方法

        /// <summary>
        /// 根据缓存名称定位需要访问的缓存服务器
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        private ClientHelper GetPooledClientManager(string cacheKey)
        {
            var nodeName = _Locator.GetPrimary(_KeyPrefix + cacheKey);
            using (var tracer = this._tracerFactory.CreateTracer($"Redis({nodeName})"))
            {
                if (_nodeClients.ContainsKey(nodeName))
                {
                    var dbs = _nodeClients[nodeName];

                    if (dbs.ContainsKey(_DbNum))
                    {
                        return dbs[_DbNum].Resolve();
                    }
                    else
                    {
                        return GetClientHelper(nodeName);
                    }
                }
                else
                {
                    return GetClientHelper(nodeName);
                }
            }
        }


        private ClientHelper GetClientHelper(string nodeName)
        {
            lock (_syncCreateClient)
            {
                if (_nodeClients.ContainsKey(nodeName))
                {
                    var dbs = _nodeClients[nodeName];

                    if (!dbs.ContainsKey(_DbNum))
                    {
                        dbs[_DbNum] = GetConnectionLoadBalancer(nodeName);
                    }
                }
                else
                {
                    var node = new Dictionary<int, ILoadBalancer<ClientHelper>>();
                    node[_DbNum] = GetConnectionLoadBalancer(nodeName);
                    _nodeClients[nodeName] = node;
                }

                return _nodeClients[nodeName][_DbNum].Resolve();
            }
        }

        private ILoadBalancer<ClientHelper> GetConnectionLoadBalancer(string nodeName)
        {
            var factory = new DefaultLoadBalancerFactory<ClientHelper>();

            return factory.Resolve(() =>
            {
                var clients = new List<ClientHelper>();
                for (int i = 0; i < this._NumberOfConnections; i++)
                {
                    clients.Add(new ClientHelper(_DbNum, _clusterConfigOptions[nodeName], _KeyPrefix));
                }
                return clients;
            });
        }

        #endregion

        #region 接口实现

        /// <summary>
        /// 缓存是否存在
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public bool KeyExists(string cacheKey)
        {
            if (!string.IsNullOrEmpty(cacheKey))
            {
                var value = GetPooledClientManager(cacheKey).StringGet(cacheKey);

                return value != null ? true : false;
            }
            return false;
        }


        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        public bool RemoveCache(string cacheKey)
        {
            return GetPooledClientManager(cacheKey).KeyDelete(cacheKey);
        }

        /// <summary>
        /// 设置缓存的过期时间
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheOutTime"></param>
        public bool ExpireEntryAt(string cacheKey, TimeSpan cacheOutTime)
        {
            return GetPooledClientManager(cacheKey).KeyExpire(cacheKey, cacheOutTime);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public T StringGet<T>(string cacheKey)
        {
            T cacheData = default(T);
            if (!string.IsNullOrEmpty(cacheKey))
            {
                cacheData = GetPooledClientManager(cacheKey).StringGet<T>(cacheKey);
            }
            return cacheData;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public async Task<T> StringGetAsync<T>(string cacheKey)
        {
            T cacheData = default(T);
            if (!string.IsNullOrEmpty(cacheKey))
            {
                return await GetPooledClientManager(cacheKey).StringGetAsync<T>(cacheKey);
            }
            return cacheData;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        public bool StringSet<T>(string cacheKey, T cacheValue)
        {
            if (!string.IsNullOrEmpty(cacheKey) && cacheValue != null)
            {
                return GetPooledClientManager(cacheKey).StringSet<T>(cacheKey, cacheValue);
            }
            return false;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        public async Task<bool> StringSetAsync<T>(string cacheKey, T cacheValue)
        {
            if (!string.IsNullOrEmpty(cacheKey) && cacheValue != null)
            {
                return await GetPooledClientManager(cacheKey).StringSetAsync<T>(cacheKey, cacheValue);
            }

            return false;
        }

        /// <summary>
        /// 设置缓存，可以加缓存过期时间
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="cacheOutTime"></param>
        public bool StringSet<T>(string cacheKey, T cacheValue, TimeSpan cacheOutTime)
        {
            if (!string.IsNullOrEmpty(cacheKey) && cacheValue != null)
            {
                if (cacheOutTime != null)
                {
                    return GetPooledClientManager(cacheKey).StringSet<T>(cacheKey, cacheValue, cacheOutTime);
                }
                else
                {
                    return StringSet<T>(cacheKey, cacheValue);
                }
            }
            return false;
        }


        /// <summary>
        /// 设置缓存，可以加缓存过期时间
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="cacheOutTime"></param>
        public async Task<bool> StringSetAsync<T>(string cacheKey, T cacheValue, TimeSpan cacheOutTime)
        {
            if (!string.IsNullOrEmpty(cacheKey) && cacheValue != null)
            {
                if (cacheOutTime != null)
                {
                    return await GetPooledClientManager(cacheKey).StringSetAsync<T>(cacheKey, cacheValue, cacheOutTime);
                }
                else
                {
                    return await StringSetAsync<T>(cacheKey, cacheValue);
                }
            }
            return false;
        }


        /// <summary>
        /// 数字递减
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public double StringDecrement(string cacheKey, double val = 1)
        {
            return GetPooledClientManager(cacheKey).StringDecrement(cacheKey);
        }


        /// <summary>
        /// 数字递减
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<double> StringDecrementAsync(string cacheKey, double val = 1)
        {
            return await GetPooledClientManager(cacheKey).StringDecrementAsync(cacheKey);
        }

        /// <summary>
        /// 数字递增
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public double StringIncrement(string cacheKey, double val = 1)
        {
            return GetPooledClientManager(cacheKey).StringIncrement(cacheKey);
        }


        /// <summary>
        /// 数字递增
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<double> StringIncrementAsync(string cacheKey, double val = 1)
        {
            return await GetPooledClientManager(cacheKey).StringIncrementAsync(cacheKey);
        }

        #region 发布订阅

        /// <summary>
        /// 发布一个事件
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public long Publish<T>(string channelId, T msg)
        {
            return GetPooledClientManager(channelId).Publish<T>(channelId, msg);
        }


        /// <summary>
        /// 订阅一个事件
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public void Subscribe<T>(string channelId, Action<T> handler)
        {
            GetPooledClientManager(channelId).Subscribe<T>(channelId, (channel, value) => { handler(value); });
        }

        public void Subscribe(string channelId, Action<object> handler)
        {
            GetPooledClientManager(channelId).Subscribe(channelId, (channel, value) => { handler(value); });
        }

        #endregion


        public double HashIncrement(string cacheKey, string dataKey, double value = 1)
        {
            return GetPooledClientManager(cacheKey).HashIncrement(cacheKey, dataKey, value);
        }

        public double HashDecrement(string cacheKey, string dataKey, double value = 1)
        {
            return GetPooledClientManager(cacheKey).HashDecrement(cacheKey, dataKey, value);
        }

        public List<T> HashKeys<T>(string cacheKey)
        {
            return GetPooledClientManager(cacheKey).HashKeys<T>(cacheKey);
        }


        public IDictionary<string, T> HashGetAll<T>(string cacheKey)
        {
            return GetPooledClientManager(cacheKey).HashGetAll<T>(cacheKey);
        }

        public T HashGet<T>(string cacheKey, string dataKey)
        {
            return GetPooledClientManager(cacheKey).HashGet<T>(cacheKey, dataKey);
        }

        public bool HashSet<T>(string cacheKey, string dataKey, T value)
        {
            return GetPooledClientManager(cacheKey).HashSet(cacheKey, dataKey, value);
        }

        public void HashSet<T>(string cacheKey, IDictionary<string, T> keyValuePairs)
        {
            GetPooledClientManager(cacheKey).HashSet(cacheKey, keyValuePairs);
        }

        public bool HashDelete(string cacheKey, params string[] dataKey)
        {
            return GetPooledClientManager(cacheKey).HashDelete(cacheKey, dataKey);
        }



        #region Lock

        public bool LockTake(string cacheKey, string value, TimeSpan expire)
        {
            return GetPooledClientManager(cacheKey).LockTake(cacheKey, value, expire);
        }

        public bool LockRelease(string cacheKey, string value)
        {
            return GetPooledClientManager(cacheKey).LockRelease(cacheKey, value);
        }

        public string LockQuery(string cacheKey)
        {
            return GetPooledClientManager(cacheKey).LockQuery(cacheKey);
        }
        #endregion Lock

        #region List

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="dbNum"></param>
        /// <returns></returns>
        public T ListLeftPop<T>(string cacheKey)
        {
            return GetPooledClientManager(cacheKey).ListLeftPop<T>(cacheKey);
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="dbNum"></param>
        public void ListLeftPush<T>(string cacheKey, T value)
        {
            GetPooledClientManager(cacheKey).ListLeftPush<T>(cacheKey, value);
        }

        /// <summary>
        /// 获取列表长度
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="dbNum"></param>
        /// <returns></returns>
        public long ListLength(string cacheKey)
        {
            return GetPooledClientManager(cacheKey).ListLength(cacheKey);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="dbNum"></param>
        /// <returns></returns>
        public List<T> ListRange<T>(string cacheKey)
        {
            return GetPooledClientManager(cacheKey).ListRange<T>(cacheKey);
        }

        /// <summary>
        /// 移除一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="dbNum"></param>
        public void ListRemove<T>(string cacheKey, T value)
        {
            GetPooledClientManager(cacheKey).ListRemove<T>(cacheKey, value);
        }


        /// <summary>
        /// 入队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="dbNum"></param>
        public void ListRightPush<T>(string cacheKey, T value)
        {
            GetPooledClientManager(cacheKey).ListRightPush<T>(cacheKey, value);
        }

        /// <summary>
        /// 出队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="dbNum"></param>
        public T ListRightPush<T>(string cacheKey)
        {
            return GetPooledClientManager(cacheKey).ListRightPop<T>(cacheKey);
        }

        /// <summary>
        /// 出队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="dbNum"></param>
        public T ListRightPopLeftPush<T>(string sourceCacheKey, string destCacheKey)
        {
            return GetPooledClientManager(sourceCacheKey).ListRightPopLeftPush<T>(sourceCacheKey, destCacheKey);
        }

        #endregion


        #region Set

        public bool SetAdd<T>(string key, T value)
        {
            return GetPooledClientManager(key).SetAdd(key, value);
        }

        public bool SetContains<T>(string key, T value)
        {
            return GetPooledClientManager(key).SetContains(key, value);
        }

        public long SetLength(string key)
        {
            return GetPooledClientManager(key).SetLength(key);
        }

        public List<T> SetMembers<T>(string key)
        {
            return GetPooledClientManager(key).SetMembers<T>(key);
        }

        public T SetPop<T>(string key)
        {
            return GetPooledClientManager(key).SetPop<T>(key);
        }

        public T SetRandomMember<T>(string key)
        {
            return GetPooledClientManager(key).SetRandomMember<T>(key);
        }

        public List<T> SetRandomMembers<T>(string key, long count)
        {
            return GetPooledClientManager(key).SetRandomMembers<T>(key, count);
        }

        public bool SetRemove<T>(string key, T value)
        {
            return GetPooledClientManager(key).SetRemove(key, value);
        }

        public long SetRemove<T>(string key, T[] values)
        {
            return GetPooledClientManager(key).SetRemove(key, values);
        }
        #endregion

        #region Script
        public dynamic Execute(string command, params object[] objs)
        {
            return GetPooledClientManager(command).Execute(command, objs);
        }

        public dynamic Execute<T>(string command, params object[] objs)
        {
            var redisResult = GetPooledClientManager(command).Execute(command, objs);
            return RedisResultHandler<T>(redisResult);
        }

        public async Task<dynamic> ExecuteAsync(string command, params object[] objs)
        {
            return await GetPooledClientManager(command).ExecuteAsync(command, objs);
        }

        public async Task<dynamic> ExecuteAsync<T>(string command, params object[] objs)
        {
            var redisResult = await GetPooledClientManager(command).ExecuteAsync(command, objs);
            return RedisResultHandler<T>(redisResult);
        }
        /// <summary>
        /// 执行一段命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns>RedisResult</returns>
        public dynamic ScriptEvaluate(string command, object parameters = null)
        {
            return GetPooledClientManager(command).ScriptEvaluate(command, parameters);
        }

        /// <summary>
        /// 执行一段命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public dynamic ScriptEvaluate<T>(string command, object parameters = null)
        {
            var redisResult = GetPooledClientManager(command).ScriptEvaluate(command, parameters);
            return RedisResultHandler<T>(redisResult);
        }

        /// <summary>
        /// 执行一段命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns>RedisResult</returns>
        public async Task<dynamic> ScriptEvaluateAsync(string command, object parameters = null)
        {
            return await GetPooledClientManager(command).ScriptEvaluateAsync(command, parameters);
        }

        /// <summary>
        /// 执行一段命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<dynamic> ScriptEvaluateAsync<T>(string command, object parameters = null)
        {
            var redisResult = await GetPooledClientManager(command).ScriptEvaluateAsync(command, parameters);
            return RedisResultHandler<T>(redisResult);
        }

        private dynamic RedisResultHandler<T>(RedisResult redisResult)
        {
            switch (redisResult.Type)
            {
                case ResultType.SimpleString:
                case ResultType.Integer:
                case ResultType.BulkString:
                    return ClientHelper.ConvertObj<T>((RedisValue)redisResult);
                case ResultType.MultiBulk:
                    return ClientHelper.ConvetList<T>((RedisValue[])redisResult);
            }
            return default(T);
        }
        #endregion

        #region 布隆过滤器
        /// <summary>
        /// 添加布隆过滤
        /// </summary>
        /// <param name="bloomFilterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool BF4ADD(string bloomFilterName, string value)
        {
            return GetPooledClientManager(bloomFilterName).BF4ADD(bloomFilterName, value);
        }
        /// <summary>
        /// 添加布隆过滤
        /// </summary>
        /// <param name="bloomFilterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> BF4ADDAsync(string bloomFilterName, string value)
        {
            return await GetPooledClientManager(bloomFilterName).BF4ADDAsync(bloomFilterName, value);
        }
        /// <summary>
        /// 是否存在于布隆过滤
        /// </summary>
        /// <param name="bloomFilterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool BF4EXISTS(string bloomFilterName, string value)
        {
            return GetPooledClientManager(bloomFilterName).BF4EXISTS(bloomFilterName, value);
        }
        /// <summary>
        /// 是否存在于布隆过滤
        /// </summary>
        /// <param name="bloomFilterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> BF4EXISTSAsync(string bloomFilterName, string value)
        {
            return (await GetPooledClientManager(bloomFilterName).BF4EXISTSAsync(bloomFilterName, value));
        }

        #endregion

        #endregion
    }
}
