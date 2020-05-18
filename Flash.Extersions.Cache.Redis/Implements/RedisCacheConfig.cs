using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extersions.Cache.Redis
{
    /// <summary>
    /// Redis服务配置
    /// </summary>
    public class RedisCacheConfig
    {
        public RedisCacheConfig WithNumberOfConnections(int NumberOfConnections)
        {
            this.NumberOfConnections = NumberOfConnections;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReadServerList">192.168.100.51:16378,192.168.100.51:26378 或 node1@191.168.0.1.16378,node2@191.168.0.1.16378,node1@191.168.0.1.26378,node2@191.168.0.1.26378</param>
        public RedisCacheConfig WithReadServerList(string ReadServerList)
        {
            this.ReadServerList = ReadServerList;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WriteServerList">127.0.0.1:6378 或 node1@191.168.0.1.6378,node2@191.168.0.1.6378</param>
        public RedisCacheConfig WithWriteServerList(string WriteServerList)
        {
            this.WriteServerList = WriteServerList;
            return this;
        }

        /// <summary>
        /// 哨兵列表
        /// </summary>
        public RedisCacheConfig WithSentineList(string SentineList)
        {
            this.SentineList = SentineList;
            return this;

        }
        public RedisCacheConfig WithPassword(string Password)
        {
            this.Password = Password;
            return this;
        }
        public RedisCacheConfig WithKeyPrefix(string KeyPrefix)
        {
            this.KeyPrefix = KeyPrefix;
            return this;
        }

        public RedisCacheConfig WithSsl(bool Ssl)
        {
            this.Ssl = Ssl;
            return this;
        }

        public RedisCacheConfig WithDb(int num)
        {
            this.DBNum = num;
            return this;
        }

        /// <summary>
        /// 读服务器列表
        /// </summary>
        internal string ReadServerList { get; private set; }
        /// <summary>
        /// 写入服务器列表
        /// </summary>
        internal string WriteServerList { get; private set; }
        /// <summary>
        /// 哨兵列表
        /// </summary>
        internal string SentineList { get; private set; }
        /// <summary>
        /// 密码
        /// </summary>
        internal string Password { get; private set; }
        /// <summary>
        /// Key前缀
        /// </summary>
        internal string KeyPrefix { get; private set; }
        /// <summary>
        /// 是否SSL连接
        /// </summary>
        internal bool Ssl { get; private set; } = false;
        /// <summary>
        /// 默认数据库
        /// </summary>
        internal int DBNum { get; private set; } = 0;
        /// <summary>
        /// 连接数
        /// </summary>
        internal int NumberOfConnections { get; private set; } = 10;

    }
}
