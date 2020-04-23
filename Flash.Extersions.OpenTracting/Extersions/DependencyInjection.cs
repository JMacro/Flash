using Flash.Core;
using Flash.Extersions.OpenTracting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加日志追踪
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddLinkTrack(this IFlashHostBuilder hostBuilder)
        {
            hostBuilder.Services.AddSingleton<ILinkTrack, LinkTrack>();
            return hostBuilder;
        }
    }
}
