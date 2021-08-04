using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Threading.Tasks;

namespace Flash.Extensions.ORM.EntityFrameworkCore
{
    public interface IUnitOfWork
    {
        bool SaveChanges();
        Task<bool> SaveChangesAsync();
        IDbContextTransaction BeginTransaction();
        IDbConnection GetConnection();
    }
}
