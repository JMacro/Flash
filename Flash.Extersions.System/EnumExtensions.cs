using System.ComponentModel;
using System.Reflection;

namespace System
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举类型描述 
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static string GetEnumDescript(this Type enumType)
        {
            DescriptionAttribute attr = null;
            attr = (DescriptionAttribute)Attribute.GetCustomAttribute(enumType, typeof(DescriptionAttribute));
            if (attr != null && !string.IsNullOrEmpty(attr.Description))
            {
                return attr.Description;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据枚举项的值获取枚举项的描述信息。
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns></returns>
        public static string GetEnumDescript(this object enumValue)
        {
            Type enumType = enumValue.GetType();
            DescriptionAttribute attr = null;

            // 获取枚举常数名称。
            string name = System.Enum.GetName(enumType, enumValue);
            if (name != null)
            {
                // 获取枚举字段。
                FieldInfo fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    // 获取描述的属性。
                    attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) as DescriptionAttribute;
                }
            }
            if (attr != null && !string.IsNullOrEmpty(attr.Description))
            {
                return attr.Description;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetEnumDescript(string enumValue, Type enumType)
        {
            int value = 0;
            try
            {
                value = Convert.ToInt32(enumValue);
            }
            catch
            {
                return enumValue;
            }

            DescriptionAttribute attr = null;

            // 获取枚举常数名称。
            string name = System.Enum.GetName(enumType, value);
            if (name != null)
            {
                // 获取枚举字段。
                FieldInfo fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    // 获取描述的属性。
                    attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) as DescriptionAttribute;
                }
            }
            if (attr != null && !string.IsNullOrEmpty(attr.Description))
            {
                return attr.Description;
            }
            else
            {
                return string.Empty;
            }
        }



        /// <summary>
        /// 根据枚举类型和项的值获取枚举项的名称。
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="enumValue">枚举值</param>
        /// <returns></returns>
        public static string GetEnumFieldName(this Type enumType, object enumValue)
        {
            string name = System.Enum.GetName(enumType, enumValue);
            if (name != null)
            {
                return name;
            }
            else
            {
                return string.Empty;
            }
        }

    }
}
