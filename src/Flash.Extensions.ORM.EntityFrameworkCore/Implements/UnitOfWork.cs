using Flash.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Flash.Extensions.ORM.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BaseDbContext _context;
        private IDbContextTransaction _dbContextTransaction;
        private bool _disposed = false;
        public bool IsTransaction { get; private set; }

        public UnitOfWork(BaseDbContext context)
        {
            _context = context;
#if DEBUG
            var logger = MicrosoftContainer.Instance.GetService<ILogger<UnitOfWork>>();
            logger.LogInformation($"DbContextId:{_context.ContextId}");
#endif
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public IDbContextTransaction BeginTransaction()
        {
            this.IsTransaction = true;
            this._dbContextTransaction = _context.Database.BeginTransaction();
            return this._dbContextTransaction;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }

            if (this.IsTransaction && _dbContextTransaction != null)
            {
                this.IsTransaction = false;
                this._dbContextTransaction.Dispose();
            }

            _disposed = true;
        }

        public IDbConnection GetConnection()
        {
            return _context.Database.GetDbConnection();
        }

        public bool Commit()
        {
            try
            {
                this._dbContextTransaction.Commit();
                return true;
            }
            catch (Exception)
            {
                this._dbContextTransaction.Rollback();
                return false;
            }
        }

        public async Task<bool> CommitAsync()
        {
            try
            {
                await this._dbContextTransaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await this._dbContextTransaction.RollbackAsync();
                return false;
            }
        }
    }

    public class UnitOfWork<TDbContext> : UnitOfWork, IUnitOfWork<TDbContext> where TDbContext : BaseDbContext
    {
        public UnitOfWork(TDbContext context) : base(context)
        {
        }
    }
}
