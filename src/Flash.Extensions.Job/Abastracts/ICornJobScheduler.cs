using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Job
{
    /// <summary>
    /// Job执行器
    /// </summary>
    public interface ICornJobScheduler : IDisposable
    {
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RunAsync(CancellationToken cancellationToken);
        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ShutdownAsync(CancellationToken cancellationToken);
    }
}
