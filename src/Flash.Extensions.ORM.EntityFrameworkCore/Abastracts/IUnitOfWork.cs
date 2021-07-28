using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Threading.Tasks;

namespace Flash.Extensions.ORM.EntityFrameworkCore
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();        
        IDbContextTransaction BeginTransaction();
        IDbConnection GetConnection();
    }
}
