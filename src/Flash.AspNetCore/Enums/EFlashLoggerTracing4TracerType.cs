using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.AspNetCore.Enums
{
    public enum EFlashLoggerTracing4TracerType
    {
        /// <summary>
        /// 未配置
        /// </summary>
        None = 0,
        /// <summary>
        /// Jaeger
        /// </summary>
        Jaeger = 1,
        /// <summary>
        /// Skywalking
        /// </summary>
        Skywalking = 2
    }
}
