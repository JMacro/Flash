using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Flash.Extensions.ORM.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        private bool _disposed = false;
        private bool _isTransaction = false;

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public bool SaveChanges()
        {
            if (!_isTransaction)
            {
                return _context.SaveChanges() > 0;
            }
            return true;
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (!_isTransaction)
            {
                return await _context.SaveChangesAsync() > 0;
            }
            return true;
        }

        public IDbContextTransaction BeginTransaction()
        {
            this._isTransaction = true;
            return _context.Database.BeginTransaction();
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

            _disposed = true;
        }

        public IDbConnection GetConnection()
        {
            return _context.Database.GetDbConnection();
        }
    }
}
