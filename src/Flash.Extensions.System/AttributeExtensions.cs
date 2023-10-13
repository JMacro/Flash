using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Flash.Extensions
{
    public static class AttributeExtensions
    {
        /// <summary>
        /// 是否为指定的Attribute
        /// </summary>
        /// <typeparam name="T">Attribute</typeparam>
        /// <param name="customAttributes"></param>
        /// <returns></returns>
        public static bool Is<T>(this IEnumerable<CustomAttributeData> customAttributes) where T : Attribute
        {
            return customAttributes.Any(p => Is<T>(p));
        }

        /// <summary>
        /// 是否为指定的Attribute
        /// </summary>
        /// <typeparam name="T">Attribute</typeparam>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static bool Is<T>(this Attribute[] attributes) where T : Attribute
        {
            return attributes.Any(p => Is<T>(p));
        }

        /// <summary>
        /// 是否为指定的Attribute
        /// </summary>
        /// <typeparam name="T">Attribute</typeparam>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static bool Is<T>(this Attribute attributes) where T : Attribute
        {
            return attributes.GetType() == typeof(T);
        }

        /// <summary>
        /// 是否为指定的Attribute
        /// </summary>
        /// <typeparam name="T">Attribute</typeparam>
        /// <param name="customAttribute"></param>
        /// <returns></returns>
        public static bool Is<T>(this CustomAttributeData customAttribute) where T : Attribute
        {
            return customAttribute.AttributeType == typeof(T);
        }
    }
}
