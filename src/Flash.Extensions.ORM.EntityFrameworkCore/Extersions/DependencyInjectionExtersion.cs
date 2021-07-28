using Flash.Extensions.ORM;
using Flash.Extensions.ORM.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

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
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}
