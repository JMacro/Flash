using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Newtonsoft.Json
{
    public class DesensitizationConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return AsType(reader.Value?.ToString(), objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = value.GetType();
            Func<Attribute[], Type, bool> IsMyAttribute = (tageAttributes, myAttributeType) =>
            {
                foreach (Attribute a in tageAttributes)
                {
                    if (a.GetType() == myAttributeType)
                        return true;
                }
                return false;
            };

            var jo = new JObject();
            foreach (PropertyInfo propInfo in type.GetProperties())
            {
                if (propInfo.CanRead)
                {
                    var propVal = propInfo.GetValue(value, null);
                    if (IsMyAttribute(System.Attribute.GetCustomAttributes(propInfo, true), typeof(DesensitizationAttribute)))
                    {
                        var attribute = Attribute.GetCustomAttributes(propInfo, true).FirstOrDefault(f => f is DesensitizationAttribute) as DesensitizationAttribute;
                        var val = attribute.Begin(propVal, propInfo.PropertyType);

                        var typeCode = Type.GetTypeCode(propInfo.PropertyType.Name.Equals("Nullable`1") ? propInfo.PropertyType.GetGenericArguments().First() : propInfo.PropertyType);

                        if (!propInfo.PropertyType.IsValueType && typeCode == TypeCode.Object)
                        {
                            jo.Add(propInfo.Name, val != null ? JToken.FromObject(val, serializer) : null);
                        }
                        else
                        {
                            jo.Add(propInfo.Name, val != null ? JToken.FromObject(val) : null);
                        }
                    }
                    else
                    {
                        jo.Add(propInfo.Name, propVal != null ? JToken.FromObject(propVal, serializer) : null);
                    }
                }
            }
            jo.WriteTo(writer);
        }

        /// <summary>
        /// 字符串格式数据转其他类型数据
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="destinationType">目标格式</param>
        /// <returns>转换结果</returns>
        public static object AsType(string input, Type destinationType)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(destinationType);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    return converter.ConvertFrom(null, null, input);
                }

                converter = TypeDescriptor.GetConverter(typeof(string));
                if (converter.CanConvertTo(destinationType))
                {
                    return converter.ConvertTo(null, null, input, destinationType);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
    }

    /// <summary>
    /// 数据脱敏
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class DesensitizationAttribute : Attribute
    {
        private readonly int _beginMaskIndex;
        private readonly int _maskLength;

        public DesensitizationAttribute(int beginMaskIndex = -1, int maskLength = 0)
        {
            this._beginMaskIndex = beginMaskIndex;
            this._maskLength = maskLength;
        }

        public object Begin(object value, Type type)
        {
            if (type.IsValueType || type.FullName == "System.String")
            {
                return value?.ToString().ToMask(this._beginMaskIndex, this._maskLength);
            }
            return value;
        }
    }
}
