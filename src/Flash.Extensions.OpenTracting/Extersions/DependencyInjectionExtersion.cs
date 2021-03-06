﻿using Flash.Core;
using Flash.Extensions.OpenTracting;
using System;

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
