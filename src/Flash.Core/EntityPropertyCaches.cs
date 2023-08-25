using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Flash.Core
{
    public static class EntityPropertyCaches
    {
        private static object _Lock = new object();
        /// <summary>
        /// 实体属性信息集合
        /// </summary>
        private static ConcurrentDictionary<Type, IList<PropertyInfo>> CacheEntityPropertys = new ConcurrentDictionary<Type, IList<PropertyInfo>>();

        /// <summary>
        /// 尝试获得属性信息集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<PropertyInfo> TryGetOrAddByProperties(Type type)
        {
            lock (_Lock)
            {
                if (!CacheEntityPropertys.TryGetValue(type, out var entityType))
                {
                    var properties = type.GetProperties();
                    entityType = CacheEntityPropertys.GetOrAdd(type, properties);
                }
                return entityType;
            }
        }
    }
}
