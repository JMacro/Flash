using Flash.Core;
using Flash.Extersions.EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加事件总线
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddEventBus(this IFlashHostBuilder hostBuilder, Action<IEventBusHostBuilder> setup)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(a => a.GetTypes().Where(type => Array.Exists(type.GetInterfaces(), t => t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(IProcessMessageHandler<>)))))
                       .ToArray();

            foreach (var type in types)
            {
                hostBuilder.Services.AddSingleton(type);
            }

            var builder = new EventBusHostBuilder(hostBuilder.Services);
            setup(builder);

            return hostBuilder;
        }

#if NETCORE
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFlashApplicationBuilder UseEventBus(this IFlashApplicationBuilder builder,Action<IServiceProvider> action)
        {
            action(builder.app.ApplicationServices);
            return builder;
        }
#endif

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IFlashApplicationBuilder UseSubscriber(this IFlashApplicationBuilder appBuilder, Action<IEventBus> setup)
        {
            var eventBus = appBuilder.app.ApplicationServices.GetRequiredService<IEventBus>();
            var logger = appBuilder.app.ApplicationServices.GetRequiredService<ILogger<IEventBus>>();

            eventBus.Subscriber((Messages) =>
            {
                foreach (var message in Messages)
                {
                    logger.LogDebug($"ACK: queue {message.QueueName} route={message.RouteKey} messageId:{message.MessageId}");
                }

            }, async (obj) =>
            {
                foreach (var message in obj.Messages)
                {
                    logger.LogError($"NAck: queue {message.QueueName} route={message.RouteKey} messageId:{message.MessageId}");
                }

                //消息消费失败执行以下代码
                if (obj.Exception != null)
                {
                    logger.LogError(obj.Exception, obj.Exception.Message);
                }

                return true;
            });

            setup?.Invoke(eventBus);
            return appBuilder;
        }


    }
}
