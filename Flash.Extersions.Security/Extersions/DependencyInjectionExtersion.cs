using Flash.Core;
using Flash.Extersions.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public class SecurityOption
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string SecretKey { get; set; } = "";
        /// <summary>
        /// 加\解密编码
        /// </summary>
        public Encoding Encoding { get; set; } = null;
    }

    public static partial class DependencyInjectionExtersion
    {
        public static IFlashHostBuilder AddSecurity3DES(this IFlashHostBuilder hostBuilder, Action<SecurityOption> setup)
        {
            var option = new SecurityOption();
            setup(option);

            hostBuilder.Services.AddSingleton<ISecurity3DES>(sp =>
            {
                return new Security3DES(option.SecretKey, option.Encoding);
            });
            return hostBuilder;
        }
    }
}
