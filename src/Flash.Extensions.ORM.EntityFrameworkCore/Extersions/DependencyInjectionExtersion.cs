using Flash.Extensions.ORM;
using Flash.Extensions.ORM.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 注册EFCore仓储
        /// </summary>
        /// <param name="cacheBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFlashOrmBuilder UseEFCore<TDbContext>(this IFlashOrmBuilder ormBuilder, Action<DbContextOptionsBuilder> options) where TDbContext : BaseDbContext
        {
            ormBuilder.Services.AddDbContext<TDbContext>(options);
            ormBuilder.Services.AddScoped<DbContext, TDbContext>();
            AddDefault(ormBuilder.Services);
            return ormBuilder;
        }

        private static void AddDefault(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            AutoDi(services);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

        private static IServiceCollection AutoDi(this IServiceCollection services)
        {
            List<Assembly> currentPathAssembly = AppDomain.CurrentDomain.GetCurrentPathAssembly();
            foreach (Assembly item2 in currentPathAssembly)
            {
                IEnumerable<Type> enumerable = from type in item2.GetTypes()
                                               where type.IsClass && type.BaseType != null && type.HasImplementedRawGeneric(typeof(IDependency))
                                               select type;
                foreach (Type type2 in enumerable)
                {
                    Type[] interfaces = type2.GetInterfaces();
                    Type type3 = interfaces.FirstOrDefault((Type x) => x.Name == "I" + type2.Name);
                    if (type3 == null)
                    {
                        type3 = type2;
                    }

                    ServiceDescriptor item = new ServiceDescriptor(type3, type2, ServiceLifetime.Scoped);
                    if (!services.Contains(item))
                    {
                        services.Add(item);
                    }
                }
            }

            return services;
        }

        private static List<Assembly> GetCurrentPathAssembly(this AppDomain domain)
        {
            List<CompilationLibrary> list = DependencyContext.Default.CompileLibraries.Where((CompilationLibrary x) => !x.Name.StartsWith("Microsoft") && !x.Name.StartsWith("System")).ToList();
            List<Assembly> list2 = new List<Assembly>();
            if (list.Any())
            {
                foreach (CompilationLibrary item in list)
                {
                    if (item.Type == "project")
                    {
                        list2.Add(Assembly.Load(item.Name));
                    }
                }
            }

            return list2;
        }

        private static bool HasImplementedRawGeneric(this Type type, Type generic)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (generic == null)
            {
                throw new ArgumentNullException("generic");
            }

            if (type.GetInterfaces().Any(IsTheRawGenericType))
            {
                return true;
            }

            while (type != null && type != typeof(object))
            {
                if (IsTheRawGenericType(type))
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
            bool IsTheRawGenericType(Type test)
            {
                return generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
            }
        }
    }
}
