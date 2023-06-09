using Flash.Core;
using Flash.Extensions.ChangeHistory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加实体变更日志记录
        /// <para><see cref="IEntityChange"/></para>
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddEntityChange(this IFlashHostBuilder hostBuilder, Action<IEntityChangeHostBuilder> setup)
        {
            var builder = new EntityChangeHostBuilder(hostBuilder.Services);
            setup(builder);

            hostBuilder.Services.TryAddSingleton<IEntityChange>((sp) =>
            {
                var logger = sp.GetService<ILogger<EntityChange>>();
                var storage = sp.GetService<IStorage>();
                if (storage == null)
                {
                    storage = new DefaultStorage(sp.GetService<ILogger<DefaultStorage>>());
                }
                return new EntityChange(storage);
            });

            return hostBuilder;
        }
    }
}
