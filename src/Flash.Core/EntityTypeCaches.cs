using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Flash.Core
{
    public static class EntityTypeCaches
    {
        private static object _Lock_CacheEntityProperty = new object();
        private static object _Lock_CacheEntityTypes = new object();
        /// <summary>
        /// 实体属性信息集合
        /// </summary>
        private static ConcurrentDictionary<Type, IList<PropertyInfo>> CacheEntityPropertys = new ConcurrentDictionary<Type, IList<PropertyInfo>>();
        private static ConcurrentDictionary<string, Type> CacheEntityTypes = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// 尝试获得属性信息集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<PropertyInfo> TryGetOrAddByProperties(Type type)
        {
            lock (_Lock_CacheEntityProperty)
            {
                if (!CacheEntityPropertys.TryGetValue(type, out var entityType))
                {
                    var properties = type.GetProperties();
                    entityType = CacheEntityPropertys.GetOrAdd(type, properties);
                }
                return entityType;
            }
        }

        /// <summary>
        /// 尝试获得属性信息集合
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        public static Type TryGetOrAddByType(string typeFullName)
        {
            lock (_Lock_CacheEntityTypes)
            {
                if (!CacheEntityTypes.TryGetValue(typeFullName, out var entityType))
                {
                    entityType = Type.GetType(typeFullName);
                    CacheEntityTypes.TryAdd(typeFullName, entityType);
                }
                return entityType;
            }
        }
    }
}
