using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.Tracting
{
    /// <summary>
    /// 链路追踪
    /// </summary>
    public interface ITracer : IDisposable
    {
        /// <summary>
        /// 追踪器名称
        /// </summary>
        /// <param name="tracerName"></param>
        void SetTracerName(string tracerName);
        /// <summary>
        /// 记录请求数据
        /// </summary>
        /// <param name="value"></param>
        void LogRequest(dynamic value);
        /// <summary>
        /// 记录响应数据
        /// </summary>
        /// <param name="value"></param>
        void LogResponse(dynamic value);
        /// <summary>
        /// 记录异常数据
        /// </summary>
        /// <param name="ex"></param>
        void LogException(Exception ex);        
        /// <summary>
        /// 自定义日志记录
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Log(string key, dynamic value);
        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetTag(string key, dynamic value);
        /// <summary>
        /// 设置异常
        /// </summary>
        void SetError();
    }
}
