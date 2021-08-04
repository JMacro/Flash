using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System;
using System.Threading.Tasks;

namespace Flash.Extensions.ORM.EntityFrameworkCore
{
    public interface IUnitOfWork : IDisposable
    {
        bool SaveChanges();
        Task<bool> SaveChangesAsync();
        IDbContextTransaction BeginTransaction();
        IDbConnection GetConnection();
        bool Commit();
        Task<bool> CommitAsync();
    }
}
