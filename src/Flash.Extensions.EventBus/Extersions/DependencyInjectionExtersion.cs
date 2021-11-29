using Flash.Core;
using Flash.Extensions.EventBus;
using Flash.Extensions.EventBus.Dashboard;
using Flash.Extensions.Tracting;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
                hostBuilder.Services.TryAddSingleton(type);
            }

            hostBuilder.Services.TryAddSingleton<ITracerFactory, TracerFactory>();
            hostBuilder.Services.TryAddSingleton(_ => DashboardRoutes.Routes);

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
        public static IFlashApplicationBuilder UseEventBus(this IFlashApplicationBuilder builder, Action<IServiceProvider> action)
        {
            action(builder.app.ApplicationServices);
            return builder;
        }
#endif

        /// <summary>
        /// 使用消息事件总线订阅
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IServiceProvider UseSubscriber(this IServiceProvider serviceProvider, Action<IEventBus> setup)
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            var logger = serviceProvider.GetRequiredService<ILogger<IEventBus>>();

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
            return serviceProvider;
        }


    }
}
