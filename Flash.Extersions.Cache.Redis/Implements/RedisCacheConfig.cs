namespace Flash.Extersions.Cache.Redis
{
    /// <summary>
    /// Redis服务配置
    /// </summary>
    public class RedisCacheConfig : ICacheConfig
    {
        /// <summary>
        /// 连接数
        /// </summary>
        /// <param name="NumberOfConnections"></param>
        /// <returns></returns>
        public ICacheConfig WithNumberOfConnections(int NumberOfConnections)
        {
            this.NumberOfConnections = NumberOfConnections;
            return this;
        }

        /// <summary>
        /// 读服务列表
        /// </summary>
        /// <param name="ReadServerList">192.168.100.51:16378,192.168.100.51:26378 或 node1@191.168.0.1.16378,node2@191.168.0.1.16378,node1@191.168.0.1.26378,node2@191.168.0.1.26378</param>
        public ICacheConfig WithReadServerList(string ReadServerList)
        {
            this.ReadServerList = ReadServerList;
            return this;
        }

        /// <summary>
        /// 写服务列表
        /// </summary>
        /// <param name="WriteServerList">127.0.0.1:6378 或 node1@191.168.0.1.6378,node2@191.168.0.1.6378</param>
        public ICacheConfig WithWriteServerList(string WriteServerList)
        {
            this.WriteServerList = WriteServerList;
            return this;
        }

        /// <summary>
        /// 哨兵列表
        /// </summary>
        public ICacheConfig WithSentineList(string SentineList)
        {
            this.SentineList = SentineList;
            return this;
        }

        /// <summary>
        /// 密码
        /// </summary>
        /// <param name="Password"></param>
        /// <returns></returns>
        public ICacheConfig WithPassword(string Password)
        {
            this.Password = Password;
            return this;
        }

        /// <summary>
        /// Key前缀
        /// </summary>
        /// <param name="KeyPrefix"></param>
        /// <returns></returns>
        public ICacheConfig WithKeyPrefix(string KeyPrefix)
        {
            this.KeyPrefix = KeyPrefix;
            return this;
        }

        /// <summary>
        /// 是否使用SSH
        /// </summary>
        /// <param name="Ssl"></param>
        /// <returns></returns>
        public ICacheConfig WithSsl(bool Ssl)
        {
            this.Ssl = Ssl;
            return this;
        }

        /// <summary>
        /// Db切换
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public ICacheConfig WithDb(int num)
        {
            this.DBNum = num;
            return this;
        }

        /// <summary>
        /// //健康检测
        /// </summary>
        /// <param name="isHealthCheck"></param>
        /// <returns></returns>
        public ICacheConfig WithHealthCheck(bool isHealthCheck)
        {
            this.HealthCheck = isHealthCheck;
            return this;
        }

        /// <summary>
        /// 分布式锁，实例对象IDistributedLock
        /// </summary>
        /// <param name="isDistributedLock"></param>
        /// <returns></returns>
        public ICacheConfig WithDistributedLock(bool isDistributedLock)
        {
            this.DistributedLock = isDistributedLock;
            return this;
        }

        /// <summary>
        /// 读服务器列表
        /// </summary>
        public string ReadServerList { get; private set; }
        /// <summary>
        /// 写入服务器列表
        /// </summary>
        public string WriteServerList { get; private set; }
        /// <summary>
        /// 哨兵列表
        /// </summary>
        public string SentineList { get; private set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; private set; }
        /// <summary>
        /// Key前缀
        /// </summary>
        public string KeyPrefix { get; private set; }
        /// <summary>
        /// 是否SSL连接
        /// </summary>
        public bool Ssl { get; private set; } = false;
        /// <summary>
        /// 默认数据库
        /// </summary>
        public int DBNum { get; private set; } = 0;
        /// <summary>
        /// 连接数
        /// </summary>
        public int NumberOfConnections { get; private set; } = 10;
        /// <summary>
        /// 健康检测
        /// </summary>
        public bool HealthCheck { get; private set; } = false;
        /// <summary>
        /// 分布式锁
        /// </summary>
        public bool DistributedLock { get; private set; } = false;        
    }
}
