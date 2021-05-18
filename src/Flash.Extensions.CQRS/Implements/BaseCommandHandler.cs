using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.CQRS
{
    /// <summary>
    /// 基础命令处理基类
    /// </summary>
    /// <typeparam name="TRequest">继承BaseCommand的实体类</typeparam>
    /// <typeparam name="TResponse">响应实体</typeparam>
    public abstract class BaseCommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : BaseCommand<TResponse>
    {
        public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            throw new System.Exception("继承BaseCommandHandler后，请重写Handle方法");
        }
    }
}
