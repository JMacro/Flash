using Flash.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flash.Extensions.ORM.EntityFrameworkCore
{
    /// <summary>
    /// Db上下文
    /// <para>如需自动映射实体到上下文Model，请为实体实现<see cref="IEntity"/>接口</para>
    /// </summary>
    public class BaseDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IRegisterEvents _registerEvents;

        protected BaseDbContext()
        {
        }

        public BaseDbContext(DbContextOptions options, ILoggerFactory loggerFactory = null, IRegisterEvents registerEvents = null) : base(options)
        {
            if (loggerFactory == null) loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole().AddDebug(); });
            this._loggerFactory = loggerFactory;
            this._registerEvents = registerEvents;

            this.ChangeTracker.StateChanged += ChangeTracker_StateChanged;
#if NET6_0
            this.SavingChanges += BaseDbContext_SavingChanges;
            this.SavedChanges += BaseDbContext_SavedChanges;
            this.SaveChangesFailed += BaseDbContext_SaveChangesFailed;
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<Assembly> currentPathAssembly = GetCurrentPathAssembly();
            foreach (Assembly item in currentPathAssembly)
            {
                IEnumerable<Type> enumerable = from type in item.GetTypes()
                                               where !string.IsNullOrWhiteSpace(type.Namespace)
                                               where type.IsClass
                                               where type.BaseType != null
                                               where !type.Name.StartsWith("EntityBase")
                                               where typeof(IEntity).IsAssignableFrom(type)
                                               select type;
                foreach (Type item2 in enumerable)
                {
                    if (modelBuilder.Model.FindEntityType(item2) == null)
                    {
                        modelBuilder.Model.AddEntityType(item2);
                    }
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }

        private List<Assembly> GetCurrentPathAssembly()
        {
            List<CompilationLibrary> list = DependencyContext.Default.CompileLibraries.Where((CompilationLibrary x) => !x.Name.StartsWith("Microsoft") && !x.Name.StartsWith("System")).ToList();
            List<Assembly> list2 = new List<Assembly>();
            if (list.Any())
            {
                foreach (CompilationLibrary item in list)
                {
                    if (item.Type == "project")
                    {
                        list2.Add(Assembly.Load(item.Name));
                    }
                }
            }

            return list2;
        }

#if NET6_0
        private void BaseDbContext_SavingChanges(object sender, SavingChangesEventArgs e)
        {
            if (_registerEvents != null) _registerEvents.SavingChanges?.Invoke();
        }

        private void BaseDbContext_SaveChangesFailed(object sender, SaveChangesFailedEventArgs e)
        {
            if (_registerEvents != null) _registerEvents.SaveChangesFailed?.Invoke();
        }

        private void BaseDbContext_SavedChanges(object sender, SavedChangesEventArgs e)
        {
            if (_registerEvents != null) _registerEvents.SavedChanges?.Invoke();
        }
#endif

        private void ChangeTracker_StateChanged(object sender, EntityEntryEventArgs e)
        {
            if (e.Entry.State == Microsoft.EntityFrameworkCore.EntityState.Unchanged) return;

            if (_registerEvents != null)
            {
                var state = Map(e.Entry.State);
                var entityType = e.Entry.Entity.GetType();
                var originalEntity = default(Object);
                switch (state)
                {
                    case EntityState.Deleted:
                    case EntityState.Modified:
                        originalEntity = Activator.CreateInstance(entityType);
                        break;
                }

                SetValues(originalEntity, entityType, e.Entry.OriginalValues);
                _registerEvents.StateChanged?.Invoke(new EntityChangeTracker
                {
                    State = state,
                    ContextType = e.Entry.Context.GetType(),
                    OriginalEntity = originalEntity,
                    CurrentEntity = e.Entry.Entity,
                });
            }
        }

        private static void SetValues(object entity, Type entityType, PropertyValues values)
        {
            if (entity == null) return;

            foreach (var item in EntityPropertyCaches.TryGetOrAddByProperties(entityType))
            {
                item.SetValue(entity, values[item.Name]);
            }
        }

        private EntityState Map(Microsoft.EntityFrameworkCore.EntityState state)
        {
            var result = EntityState.Detached;
            switch (state)
            {
                case Microsoft.EntityFrameworkCore.EntityState.Detached:
                    result = EntityState.Detached;
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Unchanged:
                    result = EntityState.Unchanged;
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Deleted:
                    result = EntityState.Deleted;
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Modified:
                    result = EntityState.Modified;
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Added:
                    result = EntityState.Added;
                    break;
            }
            return result;
        }
    }
}
