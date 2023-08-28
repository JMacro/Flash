using Flash.Extensions.ORM;
using Flash.Extensions.ORM.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Storage;
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
        /// <param name="ormBuilder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
#if NET6_0
        [Obsolete($"该方法不再维护，请使用方法{nameof(UseEntityFramework)}")]
#endif
#if NETSTANDARD2_0
        [Obsolete("该方法不再维护，请使用方法UseEntityFramework")]
#endif
        public static IFlashOrmBuilder UseEFCore<TDbContext>(this IFlashOrmBuilder ormBuilder, Action<DbContextOptionsBuilder> options) where TDbContext : BaseDbContext
        {
            ormBuilder.Services.AddDbContext<TDbContext>(options);
            ormBuilder.Services.AddScoped<DbContext, TDbContext>();
            AddDefault(ormBuilder.Services);
            return ormBuilder;
        }

        /// <summary>
        /// 使用EntityFramework
        /// </summary>
        /// <param name="ormBuilder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IFlashOrmBuilder UseEntityFramework(this IFlashOrmBuilder ormBuilder, Action<IFlashOrmDbContextBuilder> options)
        {
            Flash.Extensions.Check.Argument.IsNotNull(options, nameof(options));
            var builder = new FlashOrmDbContextBuilder(ormBuilder.Services);
            builder.RegisterGlobalEvents(eventOption => { });
            options(builder);
            AddDefault(ormBuilder.Services);
            return ormBuilder;
        }

        /// <summary>
        /// 注册DB上下文
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <typeparam name="TMigrationAssembly"></typeparam>
        /// <param name="builder"></param>
        /// <param name="connectionString"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IFlashOrmDbContextBuilder RegisterDbContexts<TDbContext, TMigrationAssembly>(this IFlashOrmDbContextBuilder builder, string connectionString, IConfiguration configuration) where TDbContext : BaseDbContext where TMigrationAssembly : IMigrationAssembly
        {
            Flash.Extensions.Check.Argument.IsNotNull(connectionString, nameof(connectionString));
            Flash.Extensions.Check.Argument.IsNotNull(configuration, nameof(configuration));

            var migrationsAssembly = typeof(TMigrationAssembly).GetTypeInfo().Assembly.GetName().Name;
            var databaseProvider = configuration.GetSection(nameof(DatabaseProviderConfiguration)).Get<DatabaseProviderConfiguration>() ?? new DatabaseProviderConfiguration();
            switch (databaseProvider.ProviderType)
            {
                case DatabaseProviderType.SqlServer:
                    builder.Services.AddDbContext<TDbContext>(options => options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
                    break;
                case DatabaseProviderType.PostgreSQL:
                    builder.Services.AddDbContext<TDbContext>(options => options.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
                    break;
                case DatabaseProviderType.MySql:
#if NET6_0
                    builder.Services.AddDbContext<TDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), sql => sql.MigrationsAssembly(migrationsAssembly)));
#endif
#if NETSTANDARD2_0
                    builder.Services.AddDbContext<TDbContext>(options => options.UseMySql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
#endif
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseProvider.ProviderType), $@"The value needs to be one of {string.Join(", ", Enum.GetNames(typeof(DatabaseProviderType)))}.");
            }

            return builder;
        }

        /// <summary>
        /// 全局事件注册
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IFlashOrmDbContextBuilder RegisterGlobalEvents(this IFlashOrmDbContextBuilder builder, Action<IRegisterEvents> options)
        {
            var sp = builder.Services.BuildServiceProvider();
            var registerEvents = sp.GetService<IRegisterEvents>();
            if (registerEvents == null)
            {
                registerEvents = new RegisterEvents();
            }
            options(registerEvents);
            builder.Services.TryAdd(ServiceDescriptor.Singleton<IRegisterEvents>(registerEvents));
            return builder;
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
