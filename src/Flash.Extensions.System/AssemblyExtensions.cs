using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flash.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// 获得所有程序集
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="ignoreName">忽略的程序集名称（支持前缀模糊匹配）</param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetCurrentAssemblys(this AppDomain domain, params string[] ignoreName)
        {
            List<CompilationLibrary> list = DependencyContext.Default.CompileLibraries.Where((CompilationLibrary x) => !ignoreName.Any(p => x.Name.StartsWith(p))).ToList();

            if (list.Any())
            {
                foreach (CompilationLibrary item in list)
                {
                    if (item.Type == "project")
                    {
                        yield return Assembly.Load(item.Name);
                    }
                }
            }
        }
    }
}
