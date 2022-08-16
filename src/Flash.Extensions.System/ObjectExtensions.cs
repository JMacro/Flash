using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if NETCORE
using System.Net.Http;
#endif
using System.Reflection;
using System.Text;

namespace Flash.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Object转换为Boolean类型
        /// </summary>
        /// <param name="value">待转换字符串</param>
        /// <returns></returns>
        public static bool ToBoolean(this object value)
        {
            if (value == null) return false;
            return StringExtensions.ToBoolean(value.ToString(), false);
        }

        /// <summary>
        /// Object转换为Boolean类型
        /// </summary>
        /// <param name="value">待转换字符串</param>
        /// <returns></returns>
        public static bool ToBoolean(this object value, object defaultValue)
        {
            if (value == null)
            {
                if (defaultValue == null) return false;
                else return ToBoolean(defaultValue);
            }
            return StringExtensions.ToBoolean(value.ToString(), ToBoolean(defaultValue));
        }

        /// <summary>
        ///  float类型转换,如转换失败返回0
        /// </summary>
        /// <param name="value">需要转换的对象</param>
        /// <returns>将传递的参数转换成浮点类型，如果转换失败返回0</returns>
        public static float ToFloat(this object value)
        {
            return ToFloat(value, .0f);
        }

        /// <summary>
        /// float类型转换,如转换失败返回默认值
        /// </summary>
        /// <param name="value">需要转换的对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>将传递的参数转换成浮点类型，如果转换失败返回0</returns>
        public static float ToFloat(this object value, float defaultValue)
        {
            if (value == null) return defaultValue;

            return StringExtensions.ToFloat(value.ToString(), defaultValue);
        }

        /// <summary>
        /// Int32类型转换，如转换失败返回0
        /// </summary>
        /// <param name="value">需要转换的对象</param>
        /// <returns>将传递的参数转换成整形，如果转换失败返回0</returns>
        public static int ToInt(this object value)
        {
            if (value == null) return 0;
            return StringExtensions.ToInt(value.ToString(), 0);
        }

        public static int ToInt<T>(this T value) where T : Enum
        {
            if (value == null) return Int32.MinValue;
            try
            {
                int enumValue = Convert.ToInt32(value);
                return enumValue;
            }
            catch (Exception)
            {
                return Int32.MinValue;
            }
        }

        /// <summary>
        /// Int32类型转换，如转换失败返回0
        /// </summary>
        /// <param name="value">需要转换的对象</param>
        /// <returns>将传递的参数转换成整形，如果转换失败返回0</returns>
        public static int ToInt(this object value, object defaultValue)
        {
            if (value == null) return 0;
            return StringExtensions.ToInt(value.ToString(), ToInt(defaultValue));
        }

        /// <summary>
        /// Int32类型转换，如转换失败返回默认值
        /// </summary>
        /// <param name="value">需要转换的对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>将传递的参数转换成整形，如果转换失败返回指定的默认值</returns>
        public static int ToInt(this object value, int defaultValue)
        {
            int iValue = 0;
            if (value == null) return defaultValue;
            if (!int.TryParse(value.ToString(), out iValue))
            {
                iValue = defaultValue;
            }
            return iValue;
        }

        /// <summary>
        /// 得到字符串记录
        /// </summary>
        /// <param name="value">需要转换的对象</param>
        /// <returns>将传递的参数转换成字符类型返回，如果传递的参数为空则返回""</returns>
		public static string ToString(this object value, object defaultValue)
        {
            if (value == null) return "";
            return ToString(value, ToString(defaultValue));
        }

        /// <summary>
        /// 得到字符串记录
        /// </summary>
        /// <param name="value">需要转换的对象</param>
        /// <returns>将传递的参数转换成字符类型返回，如果传递的参数为空则返回""</returns>
		public static string ToString(this object value)
        {
            if (value == null) return "";
            return ToString(value, "");
        }

        /// <summary>
        /// 得到字符串记录
        /// </summary>
        /// <param name="value">需要转换的对象</param>
        /// <returns>将传递的参数转换成字符类型返回，如果传递的参数为空则返回""</returns>
        public static string ToString(this object value, string defaultValue)
        {
            if (value == null)
                return defaultValue;
            else
                return value.ToString().Trim();
        }

        /// <summary>
        ///  decimal类型转换,如转换失败返回0
        /// </summary>
        /// <param name="value">需要转换的对象</param>
        /// <returns>f返回decimal类型，,如转换失败返回0</returns>
        public static decimal ToDecimal(this object value)
        {
            if (value == null)
            {
                return 0;
            }
            return StringExtensions.ToDecimal(value.ToString(), 0, 4);
        }

        /// <summary>
        ///  decimal类型转换,如转换失败返回0
        /// </summary>
        /// <param name="value">需要转换的对象</param>
        /// <returns>f返回decimal类型，,如转换失败返回0</returns>
        public static decimal ToDecimal(this object value, object defaultValue)
        {
            if (value == null)
            {
                return ToDecimal(defaultValue);
            }
            return StringExtensions.ToDecimal(value.ToString(), ToDecimal(defaultValue), 4);
        }

        /// <summary>
        /// decimal类型转换,如转换失败返回0
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pointNum">小数点长度</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object value, int pointNum)
        {
            if (value == null)
            {
                return 0;
            }
            return StringExtensions.ToDecimal(value.ToString(), 0, pointNum);
        }

        /// <summary>
        /// decimal类型转换,如转换失败返回默认值
        /// </summary>
        /// <param name="value">需要转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回decimal类型，,如转换失败返回指定的默认值</returns>
        public static decimal ToDecimal(this object value, decimal defaultValue)
        {
            if (value == null)
            {
                return defaultValue;
            }
            return StringExtensions.ToDecimal(value.ToString(), defaultValue, 4);
        }

        /// <summary>
        /// 判断是否为Null或者空
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object obj)
        {
            if (obj == null)
                return true;
            else
            {
                string objStr = obj.ToString();
                return string.IsNullOrEmpty(objStr);
            }
        }

        /// <summary>
        /// 将对象序列化成Json字符串
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 深复制
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj) where T : class
        {
            if (obj == null)
                return null;

            return obj.ToJson().ToObject<T>();
        }

        /// <summary>
        /// long类型转换，如转换失败返回默认值
        /// </summary>
        /// <param name="value">需要转换的对象</param>
        /// <param name="defaultValue">64默认值</param>
        /// <returns>将传递的参数转换成长整形，如果转换失败返回指定的默认值</returns>
        public static long Tolong(this object value, Int64 defaultValue)
        {
            return value.ToString().Tolong(defaultValue);
        }

        /// <summary>
        /// long类型转换，如转换失败返回默认值
        /// </summary>
        /// <param name="value">需要转换的字符串</param>
        /// <returns>将传递的参数转换成长整形，如果转换失败返回指定的默认值</returns>
        public static long Tolong(this object value)
        {
            return value.Tolong(0);
        }

        /// <summary>
        /// 转换为FormData
        /// </summary>
        /// <param name="value">需要转换的对象</param>
        /// <returns></returns>
        public static Dictionary<string, string> ToFormData(this object value)
        {
            var type = value.GetType();
            var properties = type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var dic = new Dictionary<string, string>();
            ToFormData(null, value, properties, ref dic);
            return dic;
        }

#if NETCORE
        /// <summary>
        /// 转换为FormData
        /// </summary>
        /// <param name="value">需要转换的对象</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static MultipartFormDataContent ToFormData(this object value, Encoding encoding)
        {
            var formDataContent = new MultipartFormDataContent();
            var dic = value.ToFormData();
            foreach (var kv in dic)
            {
                formDataContent.Add(new StringContent(kv.Value, encoding), string.Format("\"{0}\"", kv.Key));
            }
            return formDataContent;
        }
#endif
        private static void ToFormData(string fieldName, object value, PropertyInfo[] properties, ref Dictionary<string, string> dic)
        {
            foreach (var propertie in properties)
            {
                var field = $"{propertie.Name}";
                if (!string.IsNullOrEmpty(fieldName))
                {
                    field = $"{fieldName}.{propertie.Name}";
                }

                if (propertie.PropertyType.IsValueType || propertie.PropertyType == typeof(string))
                {
                    dic.Add(field, propertie.GetValue(value)?.ToString() ?? "");
                }
                else if (propertie.PropertyType.IsArray)
                {
                    var elementType = propertie.PropertyType.GetElementType();
                    var isValueType = (elementType.IsValueType || elementType == typeof(string));

                    var objectValue = propertie.GetValue(value);
                    if (objectValue != null)
                    {
                        int count = Convert.ToInt32(objectValue.GetType().GetProperty("Length").GetValue(objectValue, null));
                        var getValueMethod = propertie.PropertyType.GetMethod("GetValue", new Type[] { typeof(Int32) });

                        for (int i = 0; i < count; i++)
                        {
                            object item = getValueMethod.Invoke(objectValue, new object[] { i });
                            if (isValueType)
                            {
                                dic.Add($"{field}[{i}]", item.ToString());
                            }
                            else
                            {
                                ToFormData($"{field}[{i}]", item, elementType.GetProperties(), ref dic);
                            }
                        }
                    }
                }
                else if (!propertie.PropertyType.IsValueType && !propertie.PropertyType.IsGenericType)
                {
                    var objectValue = propertie.GetValue(value);
                    ToFormData(field, objectValue, objectValue.GetType().GetProperties(), ref dic);
                }
                else if (propertie.PropertyType.IsGenericType)
                {
                    if (typeof(IList).IsAssignableFrom(propertie.PropertyType))
                    {
                        var genericType = propertie.PropertyType.GetGenericArguments().FirstOrDefault();
                        var isValueType = (genericType != null && genericType.IsValueType || genericType == typeof(string));

                        var subObj = propertie.GetValue(value);
                        if (subObj != null)
                        {
                            int count = Convert.ToInt32(subObj.GetType().GetProperty("Count").GetValue(subObj, null));
                            var itemProperty = subObj.GetType().GetProperty("Item");
                            for (int i = 0; i < count; i++)
                            {
                                object item = itemProperty.GetValue(subObj, new object[] { i });
                                if (isValueType)
                                {
                                    dic.Add($"{field}[{i}]", item.ToString());
                                }
                                else
                                {
                                    ToFormData($"{field}[{i}]", item, genericType.GetProperties(), ref dic);
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
