using Flash.Extensions.ORM.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flash.Test.Web.EFCore
{
    public class TestDbContext : BaseDbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AccountInfo>();
        }
    }
}
