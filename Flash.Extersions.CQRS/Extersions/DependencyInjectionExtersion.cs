using Autofac;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
        /// <summary>
        /// 注册验证器
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static ContainerBuilder RegisterValidatorBehavior(this ContainerBuilder container)
        {
            container.RegisterGeneric(typeof(Flash.Extersions.CQRS.Implements.ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            return container;
        }
    }
}
