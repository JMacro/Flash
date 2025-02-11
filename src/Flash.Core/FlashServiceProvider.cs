using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Microsoft容器
    /// </summary>
    public class MicrosoftContainer
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static IServiceProvider Instance { get; internal set; }
    }

    /// <summary>
    /// Autofac容器
    /// </summary>
    public class AutofacContainer
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static IContainer Instance { get; internal set; }
    }
}
