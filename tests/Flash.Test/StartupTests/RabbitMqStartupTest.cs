using Flash.Extensions.EventBus;
using Flash.Extensions.Office;
using Flash.Test.Events.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Extensions.EventBus.RabbitMQ;

namespace Flash.Test.StartupTests
{
    public class RabbitMqStartupTest : BaseStartupTest
    {
        public RabbitMqStartupTest(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });
            services.AddFlash(flash =>
            {
                flash.AddEventBus(bus =>
                {
                    bus.UseRabbitMQ(rabbitmq =>
                    {
                        var hostName = Environment.GetEnvironmentVariable("RabbitMQ:HostName", EnvironmentVariableTarget.Machine);
                        var port = Environment.GetEnvironmentVariable("RabbitMQ:Port", EnvironmentVariableTarget.Machine);
                        var userName = Environment.GetEnvironmentVariable("RabbitMQ:UserName", EnvironmentVariableTarget.Machine);
                        var password = Environment.GetEnvironmentVariable("RabbitMQ:Password", EnvironmentVariableTarget.Machine);
                        var virtualHost = Environment.GetEnvironmentVariable("RabbitMQ:VirtualHost", EnvironmentVariableTarget.Machine);

                        rabbitmq.WithEndPoint(hostName ?? "localhost", int.Parse(port ?? "5672"))
                        .WithPrefixName("自定义前缀")
                        .WithAuth(userName ?? "guest", password ?? "guest")
                        .WithExchange(virtualHost ?? "/", Exchange: $"{GetType().FullName}")
                        .WithSender(int.Parse(Configuration["RabbitMQ:SenderMaxConnections"] ?? "10"), int.Parse(Configuration["RabbitMQ:SenderAcquireRetryAttempts"] ?? "3"))
                        .WithReceiver(
                            ReceiverMaxConnections: int.Parse(Configuration["RabbitMQ:ReceiverMaxConnections"] ?? "5"),
                            ReveiverMaxDegreeOfParallelism: int.Parse(Configuration["RabbitMQ:ReveiverMaxDegreeOfParallelism"] ?? "5"),
                            ReceiverAcquireRetryAttempts: int.Parse(Configuration["RabbitMQ:ReceiverAcquireRetryAttempts"] ?? "3"));
                    });
                });
            });
        }

        public override void Configure(IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<IEventBus>>();
            app.UseFlash(flash =>
            {
                //flash.UseHangfire();
                flash.UseEventBus(sp =>
                {
                    sp.UseSubscriber(eventbus =>
                    {
                        eventbus.RegisterWaitAndRetry<TestEventMessage, TestEventMessageHandler>("", "");
                        eventbus.Register<TestEvent2Message, TestEvent2MessageHandler>(typeof(TestEvent2MessageHandler).FullName, "routerkey.log.*");

                        //订阅消息
                        eventbus.Subscriber((Messages) =>
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

                            var events = obj.Messages.Select(message => message.WaitAndRetry(a => 5, 3)).ToList();

                            var ret = !await eventbus.PublishAsync(events);

                            return ret;
                        });
                    });
                });
            });
        }
    }
}
