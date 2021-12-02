// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using RabbitMQ.Client;
using System;

namespace Flash.Extensions.HealthChecks
{
    public sealed class RabbitMQOption
    {
        #region Endpoint
        /// <summary>
        /// 服务器地址(默认:localhost)
        /// </summary>
        internal string HostName { get; set; } = "locahost";
        /// <summary>
        /// 端口（默认：5672）
        /// </summary>
        internal int Port { get; set; } = 5672;
        /// <summary>
        /// 仪表盘管理协议
        /// </summary>
        internal string DashboardProtocol = "http";
        /// <summary>
        /// 仪表盘管理端口（默认：15672）
        /// </summary>
        internal int DashboardPort { get; set; } = 15672;
        #endregion

        #region Auth

        /// <summary>
        /// 账号(默认:guest)
        /// </summary>
        internal string UserName { get; set; } = "guest";

        /// <summary>
        /// 密码(默认:guest)
        /// </summary>
        internal string Password { get; set; } = "guest";
        #endregion

        #region Exchange
        /// <summary>
        /// 虚拟主机(默认：/)
        /// </summary>
        internal string VirtualHost { get; set; } = "/";


        /// <summary>
        /// 交换机名称(默认：amq.topic)
        /// </summary>
        internal string Exchange { get; set; } = "amq.topic";

        /// <summary>
        /// 交换机类型（默认：topic）
        /// </summary>
        internal string ExchangeType { get; set; } = "topic";
        #endregion

        /// <summary>
        /// 终结点设置
        /// </summary>
        /// <param name="HostName">地址</param>
        /// <param name="Port">端口</param>
        public RabbitMQOption WithEndPoint(string HostName = "locahost", int Port = 5672)
        {
            this.HostName = HostName;
            this.Port = Port;
            return this;
        }

        /// <summary>
        /// 仪表盘管理设置
        /// </summary>
        /// <param name="dashboardProtocol">协议</param>
        /// <param name="dashboardPort">端口</param>
        /// <returns></returns>
        public RabbitMQOption WithDashboard(string dashboardProtocol = "http", int dashboardPort = 15672)
        {
            this.DashboardProtocol = dashboardProtocol;
            this.DashboardPort = dashboardPort;
            return this;
        }

        /// <summary>
        /// 设置认证信息
        /// </summary>
        /// <param name="UserName">账号</param>
        /// <param name="Password">密码</param>
        public RabbitMQOption WithAuth(string UserName, string Password)
        {
            this.UserName = UserName;
            this.Password = Password;
            return this;
        }

        /// <summary>
        /// 设置交换器信息
        /// </summary>
        /// <param name="VirtualHost">虚拟机</param>
        /// <param name="ExchangeType">交换器类型</param>
        /// <param name="Exchange">交换器名称</param>
        public RabbitMQOption WithExchange(string VirtualHost = "/", string ExchangeType = "topic", string Exchange = "amq.topic")
        {
            this.VirtualHost = VirtualHost;
            this.ExchangeType = ExchangeType;
            this.Exchange = Exchange;
            return this;
        }
    }

    public static class HealthCheckBuilderRabbitMQExtensions
    {
        public static HealthCheckBuilder AddRabbitMQCheck(this HealthCheckBuilder builder, string name, Action<RabbitMQOption> setup)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return AddRabbitMQCheck(builder, name, setup, builder.DefaultCacheDuration);
        }

        public static HealthCheckBuilder AddRabbitMQCheck(this HealthCheckBuilder builder, string name, Action<RabbitMQOption> setup, TimeSpan cacheDuration)
        {
            var option = new RabbitMQOption();
            setup(option);
            var factory = new ConnectionFactory();
            factory.HostName = option.HostName;
            factory.Port = option.Port;
            factory.Password = option.Password;
            factory.UserName = option.UserName;
            factory.VirtualHost = option.VirtualHost;
            factory.AutomaticRecoveryEnabled = true;
            factory.TopologyRecoveryEnabled = true;
            factory.UseBackgroundThreadsForIO = true;

            var hosts = option.HostName.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            builder.AddCheck($"RabbitMqCheck({name})", () =>
            {
                try
                {
                    using (var connection = factory.CreateConnection(hosts, "healthcheck"))
                    {
                        if (connection.IsOpen)
                        {
                            return HealthCheckResult.Healthy($"Healthy");
                        }
                        else
                        {
                            return HealthCheckResult.Unhealthy($"Unhealthy");

                        }
                    }
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy($"{ex.GetType().FullName}");
                }
            }, cacheDuration);

            return builder;
        }
    }
}

