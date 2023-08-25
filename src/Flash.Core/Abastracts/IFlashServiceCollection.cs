using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFlashServiceCollection
    {
        /// <summary>
        /// 服务集合
        /// </summary>
        IServiceCollection Services { get; }
    }
}
