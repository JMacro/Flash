using Flash.Core;
using Flash.Extensions;
using Flash.Extensions.Email;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="optionsAction"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddMailKit(this IFlashHostBuilder hostBuilder, Action<EmailConfig> optionsAction, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            Check.Argument.IsNotNull(hostBuilder, nameof(hostBuilder), "IServiceCollection is not dependency injection");
            Check.Argument.IsNotNull(optionsAction, nameof(optionsAction));

            var option = new EmailConfig();
            optionsAction.Invoke(option);

            AddProviderService(hostBuilder.Services, option, lifetime);
            return hostBuilder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="emailConfig"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddMailKit(this IFlashHostBuilder hostBuilder, EmailConfig emailConfig, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            Check.Argument.IsNotNull(hostBuilder, nameof(hostBuilder), "IServiceCollection is not dependency injection");
            Check.Argument.IsNotNull(emailConfig, nameof(emailConfig));

            AddProviderService(hostBuilder.Services, emailConfig, lifetime);
            return hostBuilder;
        }

        private static void AddProviderService(IServiceCollection serviceCollection, EmailConfig options, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            MailKitProvider provider = new MailKitProvider(options);
            serviceCollection.TryAddSingleton<IMailKitProvider>(provider);
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IEmailService), typeof(EmailService), lifetime));
        }
    }
}
