using Flash.Core;
using Flash.Extersions.OpenTracting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        public static IFlashHostBuilder AddOpenTracing(this IFlashHostBuilder hostBuilder, Action<IFlashTractingBuilder> action)
        {
            var builder = new FlashTractingBuilder(hostBuilder.Services);
            action(builder);
            return hostBuilder;
        }
    }
}
