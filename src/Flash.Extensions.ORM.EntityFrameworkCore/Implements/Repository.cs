using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Flash.Extensions.ORM.EntityFrameworkCore
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private DbSet<TEntity> _entities;
        
        public virtual IQueryable<TEntity> Table => throw new NotImplementedException();

        public virtual IQueryable<TEntity> TableNoTracking => throw new NotImplementedException();
        protected virtual DbSet<TEntity> Entities => _entities ?? (_entities = _context.Set<TEntity>());

        public Repository(DbContext context)
        {
            this._context = context;
        }

        public TEntity GetById(object id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            return Entities.Find(id);
        }

        public ValueTask<TEntity> GetByIdAsync(object id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            return Entities.FindAsync(id);
        }

        public void Insert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Entities.Add(entity);
        }

        public void Insert(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
            {
                throw new ArgumentNullException("entities");
            }

            Entities.AddRange(entities);
        }

        public void InsertAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            Entities.AddAsync(entity);
        }

        public Task InsertAsync(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
            {
                throw new ArgumentNullException("entities");
            }

            return Entities.AddRangeAsync(entities);
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Entities.Attach(entity);
            _context.Update(entity);
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
            {
                throw new ArgumentNullException("entities");
            }

            _context.UpdateRange(entities);
        }

        public void Update(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
        {
            foreach (Expression<Func<TEntity, object>> expression in properties)
            {
                string text = expression.Name;
                if (string.IsNullOrEmpty(text))
                {
                    text = GetPropertyName(expression.Body.ToString());
                }

                _context.Entry(entity).Property(text).IsModified = true;
            }
        }

        private string GetPropertyName(string str)
        {
            return str.Split(',')[0].Split('.')[1];
        }

        public void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _context.Remove(entity);
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
            {
                throw new ArgumentNullException("entities");
            }

            _context.RemoveRange(entities);
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            _context.RemoveRange(Entities.Where(predicate));
        }
    }
}
