namespace Flash.Extersions.Cache
{
    public interface ICacheConfig
    {
        /// <summary>
        /// 连接数
        /// </summary>
        /// <param name="NumberOfConnections"></param>
        /// <returns></returns>
        ICacheConfig WithNumberOfConnections(int NumberOfConnections);
        /// <summary>
        /// 读服务列表
        /// </summary>
        /// <param name="ReadServerList">192.168.100.51:16378,192.168.100.51:26378 或 node1@191.168.0.1.16378,node2@191.168.0.1.16378,node1@191.168.0.1.26378,node2@191.168.0.1.26378</param>
        ICacheConfig WithReadServerList(string ReadServerList);
        /// <summary>
        /// 写服务列表
        /// </summary>
        /// <param name="WriteServerList">127.0.0.1:6378 或 node1@191.168.0.1.6378,node2@191.168.0.1.6378</param>
        /// <returns></returns>
        ICacheConfig WithWriteServerList(string WriteServerList);
        /// <summary>
        /// 哨兵列表
        /// </summary>
        /// <param name="SentineList"></param>
        /// <returns></returns>
        ICacheConfig WithSentineList(string SentineList);
        /// <summary>
        /// 密码
        /// </summary>
        /// <param name="Password"></param>
        /// <returns></returns>
        ICacheConfig WithPassword(string Password);
        /// <summary>
        /// Key前缀
        /// </summary>
        /// <param name="KeyPrefix"></param>
        /// <returns></returns>
        ICacheConfig WithKeyPrefix(string KeyPrefix);
        /// <summary>
        /// 是否使用SSH
        /// </summary>
        /// <param name="Ssl"></param>
        /// <returns></returns>
        ICacheConfig WithSsl(bool Ssl);
        /// <summary>
        /// Db切换
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        ICacheConfig WithDb(int num);
        /// <summary>
        /// 健康检测
        /// </summary>
        /// <param name="isHealthCheck"></param>
        /// <returns></returns>
        ICacheConfig WithHealthCheck(bool isHealthCheck);
        /// <summary>
        /// 分布式锁
        /// </summary>
        /// <param name="isDistributedLock"></param>
        /// <returns></returns>
        ICacheConfig WithDistributedLock(bool isDistributedLock);

        /// <summary>
        /// 读服务器列表
        /// </summary>
        string ReadServerList { get; }
        /// <summary>
        /// 写入服务器列表
        /// </summary>
        string WriteServerList { get; }
        /// <summary>
        /// 哨兵列表
        /// </summary>
        string SentineList { get; }
        /// <summary>
        /// 密码
        /// </summary>
        string Password { get; }
        /// <summary>
        /// Key前缀
        /// </summary>
        string KeyPrefix { get; }
        /// <summary>
        /// 是否SSL连接
        /// </summary>
        bool Ssl { get; }
        /// <summary>
        /// 默认数据库
        /// </summary>
        int DBNum { get; }
        /// <summary>
        /// 连接数
        /// </summary>
        int NumberOfConnections { get; }
        /// <summary>
        /// 健康检测
        /// </summary>
        bool HealthCheck { get; }
        /// <summary>
        /// 分布式锁
        /// </summary>
        bool DistributedLock { get; }
    }
}
