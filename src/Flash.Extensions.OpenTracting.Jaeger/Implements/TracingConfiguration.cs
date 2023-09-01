using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.Tracting.Jaeger
{
    /// <summary>
    /// 链路跟踪配置
    /// </summary>
    public class TracingConfiguration
    {
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Open { get; set; } = false;

        /// <summary>
        /// 刷新周期
        /// </summary>
        public int FlushIntervalSeconds { get; set; } = 15;

        /// <summary>
        /// 服务名称
        /// </summary>
        public string SerivceName { get; set; } = "Example";

        /// <summary>
        /// 采样类型（默认：全量）
        /// </summary>
        public string SamplerType { get; set; } = "const";

        /// <summary>
        /// 记录日志
        /// </summary>
        public bool LogSpans { get; set; } = true;

        /// <summary>
        /// 终结点地址
        /// <para>建议采用设置<see cref="TracingConfiguration.AgentHost"/>与<see cref="TracingConfiguration.AgentPort"/>属性来连接日志链路服务器</para>
        /// </summary>
        public string EndPoint { get; set; }

        /// <summary>
        /// 代理端口
        /// </summary>
        public int AgentPort { get; set; } = 5775;

        /// <summary>
        /// 代理主机
        /// </summary>
        public string AgentHost { get; set; } = "localhost";
    }
}
