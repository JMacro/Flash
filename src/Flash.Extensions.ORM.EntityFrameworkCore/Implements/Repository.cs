using Flash.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Flash.Extensions.ORM.EntityFrameworkCore
{
    /// <summary>
    /// 仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly BaseDbContext _context;
        private DbSet<TEntity> _entities;

        public virtual IQueryable<TEntity> Table => Entities;

        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();
        protected virtual DbSet<TEntity> Entities => _entities ?? (_entities = _context.Set<TEntity>());

        public Repository(BaseDbContext context)
        {
            this._context = context;
#if DEBUG
            var logger = MicrosoftContainer.Instance.GetService<ILogger<Repository<TEntity>>>();
            logger.LogInformation($"DbContextId:{_context.ContextId}");
#endif
        }

        public TEntity GetById(object id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            return Entities.Find(new object[] { id });
        }

        public ValueTask<TEntity> GetByIdAsync(object id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            return Entities.FindAsync(new object[] { id });
        }

        public void Insert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            SetCreateTime(entity);
            SetModifyTime(entity);
            Entities.Add(entity);
        }

        public void Insert(params TEntity[] entities)
        {
            if (!entities.Any())
            {
                throw new ArgumentNullException("entities");
            }

            foreach (var entity in entities)
            {
                SetCreateTime(entity);
                SetModifyTime(entity);
            }
            Entities.AddRange(entities);
        }

        public Task InsertAsync(params TEntity[] entities)
        {
            if (!entities.Any())
            {
                throw new ArgumentNullException("entities");
            }

            foreach (var entity in entities)
            {
                SetCreateTime(entity);
                SetModifyTime(entity);
            }
            return Entities.AddRangeAsync(entities);
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            SetModifyTime(entity);
            Entities.Attach(entity);
            _context.Update(entity);
        }

        public void Update(params TEntity[] entities)
        {
            if (!entities.Any())
            {
                throw new ArgumentNullException("entities");
            }
            foreach (var entity in entities)
            {
                SetModifyTime(entity);
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
                SetModifyTime(entity);
                _context.Entry(entity).Property(text).IsModified = true;
            }
        }

        private string GetPropertyName(string str)
        {
            return str.Split(',')[0].Split('.')[1];
        }

        public void Delete(object id)
        {
            var entity = GetById(id);
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _context.Remove(entity);
        }

        public void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _context.Remove(entity);
        }

        public void Delete(params TEntity[] entities)
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

        private void SetCreateTime(IEntity value)
        {
            if (value is IEntity2CreateTime entity)
            {
                entity.CreateTime = DateTime.Now;
            }
        }

        private void SetModifyTime(IEntity value)
        {
            if (value is IEntity2ModifyTime entity)
            {
                entity.LastModifyTime = DateTime.Now;
            }
        }
    }
}
