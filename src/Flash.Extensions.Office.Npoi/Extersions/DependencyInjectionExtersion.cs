using Flash.Extensions.Office;
using Flash.Extensions.Office.Npoi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
        /// <summary>
        /// 使用Npoi组件
        /// </summary>
        /// <param name="cacheBuilder"></param>
        /// <returns></returns>
        public static IFlashOfficeBuilder UseNpoi(this IFlashOfficeBuilder cacheBuilder)
        {
            cacheBuilder.Services.AddSingleton<IOfficeTools, OfficeTools>();
            return cacheBuilder;
        }
    }
}