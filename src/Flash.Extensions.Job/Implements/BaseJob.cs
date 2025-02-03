using System.Threading.Tasks;

namespace Flash.Extensions.Job
{
    /// <summary>
    /// Job基类
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class BaseJob<TContext>
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="contextContainer"></param>
        /// <returns></returns>
        public abstract Task Execute(IJobExecutionContextContainer<TContext> contextContainer);
    }
}
