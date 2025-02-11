using Flash.Core;
using Flash.Extensions.ORM;
using Flash.Extensions.ORM.Implements;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加ORM
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddORM(this IFlashHostBuilder hostBuilder, Action<IFlashOrmBuilder> action)
        {
            var builder = new FlashOrmBuilder(hostBuilder.Services, hostBuilder);
            action(builder);
            return hostBuilder;
        }
    }
}
