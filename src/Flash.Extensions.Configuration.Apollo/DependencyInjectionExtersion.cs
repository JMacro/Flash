using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddApolloConfiguration(this IConfigurationBuilder builder, IConfiguration configuration, Dictionary<string, ConfigFileFormat> namespaces)
        {
            var apolloConfigurationBuilder = builder.AddApollo(configuration).AddDefault();

            foreach (var configFileFormat in namespaces)
            {
                apolloConfigurationBuilder.AddNamespace(configFileFormat.Key, configFileFormat.Value);
            }

            builder.Build();
            return builder;

        }
    }
}
