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
        /// <param name="eventBus"></param>
        /// <returns></returns>
        public static IEntityChangeHostBuilder UseRabbitMQStorage<TMessageHandler>(this IEntityChangeHostBuilder hostBuilder, IEventBus eventBus)
            where TMessageHandler : IProcessMessageHandler<ChangeHistoryInfo>, IMessageAckHandler
        {
            Check.Argument.IsNotNull(eventBus, "IEventBus", "未引用IEventBus组件");

            var sp = hostBuilder.Services.BuildServiceProvider();
            var defaultStorage = sp.GetService<IStorage>();
            var logging = sp.GetService<ILogger<IStorage>>();
            if (defaultStorage != null)
            {
                logging.LogWarning($"系统已注册{defaultStorage.GetType().FullName}寄存器，本次不再重复注册！");
            }
            else
            {
                hostBuilder.Services.TryAdd(new ServiceDescriptor(typeof(IStorage), typeof(RabbitMQStorage), ServiceLifetime.Singleton));
            }
            eventBus.RegisterWaitAndRetry<ChangeHistoryInfo, TMessageHandler>();
            return hostBuilder;
        }
    }
}
