using Flash.Core;
using Flash.Extensions.ChangeHistory;
using Flash.Extensions.EventBus;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using Flash.Extensions;
using Flash.Extensions.EventBus.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 使用RabbitMQ作为寄存器
        /// </summary>
        /// <typeparam name="TMessageHandler">消息处理程序</typeparam>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IEntityChangeHostBuilder UseRabbitMQStorage<TMessageHandler>(this IEntityChangeHostBuilder hostBuilder) where TMessageHandler : IProcessMessageHandler<ChangeHistoryInfo>, IMessageAckHandler
        {
            var serviceProvider = hostBuilder.Services.BuildServiceProvider();
            var eventBus = serviceProvider.GetService<IEventBus>();
            Check.Argument.IsNotNull(eventBus, "IEventBus", "未引用IEventBus组件");

            hostBuilder.Services.TryAddSingleton<IStorage, RabbitMQStorage>();
            eventBus.RegisterWaitAndRetry<ChangeHistoryInfo, TMessageHandler>();
            return hostBuilder;
        }
    }
}
