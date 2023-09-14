namespace Flash.DynamicRoute
{
    public interface IServiceDiscoveryProvider
    {
        /// <summary>
        /// 服务注册
        /// </summary>
        void Register();

        /// <summary>
        /// 取消注册
        /// </summary>
        void Deregister();

        /// <summary>
        /// 心跳检测
        /// </summary>
        void Heartbeat();

        string ServiceId { get; }
    }
}
