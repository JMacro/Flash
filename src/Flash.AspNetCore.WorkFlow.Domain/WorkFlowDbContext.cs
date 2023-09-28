using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs;
using Flash.Extensions.ORM;
using Flash.Extensions.ORM.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flash.AspNetCore.WorkFlow.Domain
{
    /// <summary>
    /// 工作流Db上下文
    /// </summary>
    public sealed class WorkFlowDbContext : BaseDbContext<WorkFlowDbContext>
    {
        public WorkFlowDbContext(DbContextOptions<WorkFlowDbContext> options, ILoggerFactory loggerFactory = null, IRegisterEvents registerEvents = null) :
            base(options, loggerFactory, registerEvents)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new FlowConfigEntityTypeConfiguration());
        }
    }
}
