using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extersions.RabbitMQ
{
    /// <summary>
    /// RabbitMQ持久化连接接口
    /// </summary>
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        /// <summary>
        /// 是否连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <returns></returns>
        bool TryConnect();

        /// <summary>
        /// 创建Model
        /// </summary>
        /// <returns></returns>
        IModel CreateModel();

        /// <summary>
        /// 获得Model
        /// </summary>
        /// <returns></returns>
        IModel GetModel();
    }
}
