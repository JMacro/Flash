using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flash.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Collection of numeric types.
        /// </summary>
        private static readonly List<Type> NumericTypes = new List<Type>
        {
            typeof(decimal),
            typeof(decimal?),
            typeof(int?),
            typeof(float?),
            typeof(long?),
            typeof(byte), typeof(sbyte),
            typeof(short), typeof(ushort),
            typeof(int), typeof(uint),
            typeof(long), typeof(ulong),
            typeof(float), typeof(double)
        };

        /// <summary>
        /// Check if the given type is a numeric type.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns><c>true</c> if it's numeric; otherwise <c>false</c>.</returns>
        public static bool IsNumeric(this Type type)
        {
            return NumericTypes.Contains(type);
        }

        /// <summary>
        /// 是否为可空类型
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type theType)
        {
            return (theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
        }

        public static bool HasImplementedRawGeneric(this Type type, Type generic)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (generic == null)
            {
                throw new ArgumentNullException("generic");
            }

            if (type.GetInterfaces().Any(IsTheRawGenericType))
            {
                return true;
            }

            while (type != null && type != typeof(object))
            {
                if (IsTheRawGenericType(type))
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
            bool IsTheRawGenericType(Type test)
            {
                return generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
            }
        }
    }
}
