using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JsonConfigurationExtersion
    {
        /// <summary>
        /// 设置环境变量
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="variable">环境变量名称</param>
        /// <param name="value">环境变量值</param>
        /// <returns></returns>
        public static IConfigurationBuilder SetEnvironmentVariable(this IConfigurationBuilder builder, string variable, string value)
        {
            if (string.IsNullOrWhiteSpace(variable))
            {
                throw new ArgumentException("ArgumentIsNullOrWhitespace", "variable");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("ArgumentIsNullOrWhitespace", "value");
            }

            Environment.SetEnvironmentVariable(variable, value);
            return builder;
        }

        public static IConfigurationBuilder AddJsonFileEx(this IConfigurationBuilder builder, string path)
        {
            return AddJsonFileEx(builder, provider: builder.GetFileProvider(), path: path, optional: false, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddJsonFileEx(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddJsonFileEx(builder, provider: builder.GetFileProvider(), path: path, optional: optional, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddJsonFileEx(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            return AddJsonFileEx(builder, provider: builder.GetFileProvider(), path: path, optional: optional, reloadOnChange: reloadOnChange);
        }

        public static IConfigurationBuilder AddJsonFileEx(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("ArgumentIsNullOrWhitespace", "path");
            }

            if (provider == null && Path.IsPathRooted(path))
            {
                provider = new PhysicalFileProvider(System.IO.Directory.GetCurrentDirectory());
                path = Path.GetFileName(path);
            }

            return builder.AddJsonFile(config =>
            {
                config.FileProvider = provider;
                config.Path = path;
                config.Optional = optional;
                config.ReloadOnChange = reloadOnChange;
            });
        }

        public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, Action<Flash.Extensions.Configuration.Json.JsonConfigurationSource> configureSource)
        {
            return ConfigurationExtensions.Add<Flash.Extensions.Configuration.Json.JsonConfigurationSource>(builder, configureSource);
        }
    }
}
