using System;
using Flash.Extensions.Cache;
using Flash.Extensions.Job;
using Flash.Extensions.Job.Hangfire;
using Flash.Extensions.Office;
using Flash.Extensions.UidGenerator;
using Flash.Widgets.DbContexts;
using Hangfire.Server;

namespace Flash.Widgets.Jobs
{
    public class TaoBaoPromotionFreeItemImportJob : BaseHangfireJob
    {
        private readonly IUniqueIdGenerator _uniqueIdGenerator;
        private readonly TaoBaoDbContext _dbContext;
        private readonly IOfficeTools _officeTools;
        private readonly ICacheManager _cache;

        public TaoBaoPromotionFreeItemImportJob(
            ILogger<TaoBaoPromotionFreeItemImportJob> logger,
            IUniqueIdGenerator uniqueIdGenerator,
            TaoBaoDbContext dbContext,
            IOfficeTools officeTools,
            ICacheManager cache) : base(logger)
        {
            this._uniqueIdGenerator = uniqueIdGenerator;
            this._dbContext = dbContext;
            this._officeTools = officeTools;
            this._cache = cache;
        }

        public override Task Execute(IJobExecutionContextContainer<PerformContext> contextContainer)
        {
            throw new NotImplementedException();
        }
    }
}

