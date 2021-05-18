using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.CQRS
{
    /// <summary>
    /// 基础验证器
    /// </summary>
    /// <typeparam name="TCommand">请求类</typeparam>
    public abstract class BaseAbstractValidator<TCommand> : AbstractValidator<TCommand> where TCommand : class, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseAbstractValidator()
        {
        }
    }
}
