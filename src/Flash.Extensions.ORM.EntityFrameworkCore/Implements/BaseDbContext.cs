using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flash.Extensions.ORM.EntityFrameworkCore
{
    public class BaseDbContext : DbContext
    {
        protected BaseDbContext()
        {
        }

        public BaseDbContext(DbContextOptions options)
            : base(options)
        {
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
    }
}
