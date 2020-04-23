using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extersions.CQRS
{
    /// <summary>
    /// MediatR基础命令抽象类
    /// </summary>
    /// <typeparam name="TResponse">响应实体</typeparam>
    public abstract class BaseCommand<TResponse> : IRequest<TResponse>
    {

    }
}
