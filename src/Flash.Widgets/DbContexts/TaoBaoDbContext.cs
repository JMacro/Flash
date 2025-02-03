using Flash.Extensions.ORM;
using Flash.Extensions.ORM.EntityFrameworkCore;
using Flash.Widgets.Models.TaobaoUtils;
using Microsoft.EntityFrameworkCore;

namespace Flash.Widgets.DbContexts
{
    public class TaoBaoDbContext : BaseDbContext
    {
        public TaoBaoDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(CoreEventId.DetachedLazyLoadingWarning));
            base.OnConfiguring(optionsBuilder);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            DbSet(modelBuilder);

            var entityTypes = modelBuilder.Model.GetEntityTypes();
        }

        private void DbSet(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogisticsTrackingEntity>();
            modelBuilder.Entity<SellItemOrderEntity>();
            modelBuilder.Entity<PromotionFreeItemEntity>();
        }
    }

    public class TaoBaoMigrationAssembly : IMigrationAssembly
    {

    }
}

