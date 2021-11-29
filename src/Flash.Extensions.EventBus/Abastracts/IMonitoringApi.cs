using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flash.Extensions.EventBus
{
    public interface IMonitoringApi
    {
        /// <summary>
        /// 获得队列
        /// </summary>
        /// <returns></returns>
        Task<List<QueueWithAllEnqueuedDto>> GetQueues();
        /// <summary>
        /// 获得死信队列列表
        /// </summary>
        /// <returns></returns>
        Task<List<QueueWithAllEnqueuedDto>> GetFailedQueues();
        /// <summary>
        /// 获得正常队列列表
        /// </summary>
        /// <returns></returns>
        Task<List<QueueWithAllEnqueuedDto>> GetNormalQueues();
        /// <summary>
        /// 获得重试队列列表
        /// </summary>
        /// <returns></returns>
        Task<List<QueueWithAllEnqueuedDto>> GetRetryQueues();
        /// <summary>
        /// 获得死信队列数
        /// </summary>
        /// <returns></returns>
        int GetFailedCount();
        /// <summary>
        /// 获得重试的队列数
        /// </summary>
        /// <returns></returns>
        int GetRetryCount();
        /// <summary>
        /// 获得正常队列数
        /// </summary>
        /// <returns></returns>
        int GetNormalCount();
        /// <summary>
        /// 获得消息
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        Task<List<QueueMessageDto>> GetMessages(string queueName);
    }
}
