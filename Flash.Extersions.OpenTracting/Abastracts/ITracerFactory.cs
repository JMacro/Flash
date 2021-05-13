using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extersions.OpenTracting
{
    public interface ITracerFactory
    {
        /// <summary>
        /// 创建一个链路追踪器
        /// </summary>
        /// <param name="tracerName">追踪器名称</param>
        /// <returns></returns>
        ITracer CreateTracer(string tracerName);
    }
}
