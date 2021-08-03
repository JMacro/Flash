using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Flash.Extensions.ORM
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Table { get; }
        IQueryable<TEntity> TableNoTracking { get; }
        TEntity GetById(object id);
        ValueTask<TEntity> GetByIdAsync(object id);
        void Insert(TEntity entity);
        void Insert(params TEntity[] entities);
        Task InsertAsync(params TEntity[] entities);
        void Update(TEntity entity);
        void Update(params TEntity[] entities);
        void Update(TEntity entity, params Expression<Func<TEntity, object>>[] properties);
        void Delete(TEntity entity);
        void Delete(params TEntity[] entities);
        void Delete(Expression<Func<TEntity, bool>> predicate);
    }
}
