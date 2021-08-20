using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;

namespace System
{
    public static class StringExtensions
    {
        /// <summary>
        /// String转换为Boolean类型
        /// </summary>
        /// <param name="value">待转换字符串</param>
        /// <returns></returns>
        public static bool ToBoolean(this String value)
        {
            return ToBoolean(value, false);
        }

        /// <summary>
        /// 字符串转换为Boolean类型
        /// </summary>
        /// <param name="value">待转换字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static bool ToBoolean(this String value, bool defaultValue)
        {
            if (((value != null) && (value != "")))
            {
                switch (value)
                {
                    case "0":
                        return false;
                    case "1":
                        return true;
                    default:
                        bool b;
                        if (bool.TryParse(value, out b)) return b;
                        else return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// float类型转换,如转换失败返回0
        /// </summary>
        /// <param name="value">需要转换的字符串</param>
        /// <returns>将传递的参数转换成浮点类型，如果转换失败返回0</returns>
        public static float ToFloat(this string value)
        {
            return ToFloat(value, .0f);
        }



        /// <summary>
        /// float类型转换,如转换失败返回默认值
        /// </summary>
        /// <param name="value">需要转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>将传递的参数转换成浮点类型，如果转换失败返回0</returns>
        public static float ToFloat(this string value, float defaultValue)
        {
            float iValue = 0f;
            if (!float.TryParse(value, out iValue))
            {
                iValue = defaultValue;
            }
            return iValue;
        }

        /// <summary>
        /// Int32类型转换，如转换失败返回0
        /// </summary>
        /// <param name="value">需要转换的字符串</param>
        /// <returns>将传递的参数转换成整形，如果转换失败返回0</returns>
        public static int ToInt(this string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            return ToInt(value, 0);
        }

        /// <summary>
        /// Int32类型转换，如转换失败返回默认值
        /// </summary>
        /// <param name="value">需要转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>将传递的参数转换成整形，如果转换失败返回指定的默认值</returns>
        public static int ToInt(this string value, int defaultValue)
        {
            int iValue = 0;
            if (string.IsNullOrEmpty(value)) return defaultValue;
            if (!int.TryParse(value, out iValue))
            {
                iValue = defaultValue;
            }
            return iValue;
        }

        /// <summary>
        /// decimal类型转换,如转换失败返回0
        /// </summary>
        /// <param name="value">需要转换的字符串</param>
        /// <returns>返回decimal类型，,如转换失败返回0</returns>
        public static decimal ToDecimal(this string value)
        {
            return ToDecimal(value, 0, 4);
        }

        /// <summary>
        /// decimal类型转换,如转换失败返回默认值
        /// </summary>
        /// <param name="value">需要转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回decimal类型，,如转换失败返回指定的默认值</returns>
        public static decimal ToDecimal(this string value, decimal defaultValue)
        {
            return ToDecimal(value, defaultValue, 4);
        }

        /// <summary>
        /// decimal类型转换,如转换失败返回默认值
        /// </summary>
        /// <param name="value">需要转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="pointNum">小数位数</param>
        /// <returns>返回decimal类型，,如转换失败返回指定的默认值</returns>
        public static decimal ToDecimal(this string value, decimal defaultValue, int pointNum)
        {
            decimal iValue = 0;

            if (!decimal.TryParse(value, out iValue))
            {
                iValue = defaultValue;
            }

            return Math.Round(iValue, pointNum);
        }

        /// <summary>
        /// long类型转换，如转换失败返回默认值
        /// </summary>
        /// <param name="value">需要转换的字符串</param>
        /// <param name="defaultValue">64默认值</param>
        /// <returns>将传递的参数转换成长整形，如果转换失败返回指定的默认值</returns>
        public static long Tolong(this string value, Int64 defaultValue)
        {
            long iValue = 0;
            if (string.IsNullOrEmpty(value)) return defaultValue;
            if (!long.TryParse(value, out iValue))
            {
                iValue = defaultValue;
            }
            return iValue;
        }

        /// <summary>
        /// long类型转换，如转换失败返回默认值
        /// </summary>
        /// <param name="value">需要转换的字符串</param>
        /// <param name="defaultValue">64默认值</param>
        /// <returns>将传递的参数转换成长整形，如果转换失败返回指定的默认值</returns>
        public static long Tolong(this string value)
        {
            return value.Tolong(0);
        }

        /// <summary>
        /// 将Json字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonStr">Json字符串</param>
        /// <returns></returns>
        public static T ToObject<T>(this string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        /// <summary>
        /// 将Json字符串反序列化为对象
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <param name="type">对象类型</param>
        /// <returns></returns>
        public static object ToObject(this string jsonStr, Type type)
        {
            return JsonConvert.DeserializeObject(jsonStr, type);
        }

        /// <summary>
        /// 将XML字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="xmlStr">XML字符串</param>
        /// <returns></returns>
        public static T XmlStrToObject<T>(this string xmlStr)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStr);
            string jsonJsonStr = JsonConvert.SerializeXmlNode(doc);

            return JsonConvert.DeserializeObject<T>(jsonJsonStr);
        }

        /// <summary>
        /// 将XML字符串反序列化为对象
        /// </summary>
        /// <param name="xmlStr">XML字符串</param>
        /// <returns></returns>
        public static JObject XmlStrToJObject(this string xmlStr)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStr);
            string jsonJsonStr = JsonConvert.SerializeXmlNode(doc);

            return JsonConvert.DeserializeObject<JObject>(jsonJsonStr);
        }

        /// <summary>
        /// 将Json字符串转为List'T'
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this string jsonStr)
        {
            return string.IsNullOrEmpty(jsonStr) ? null : JsonConvert.DeserializeObject<List<T>>(jsonStr);
        }

        /// <summary>
        /// 将Json字符串转为DataTable
        /// </summary>
        /// <param name="jsonStr">Json字符串</param>
        /// <returns></returns>
        public static DataTable ToDataTable(this string jsonStr)
        {
            return jsonStr == null ? null : JsonConvert.DeserializeObject<DataTable>(jsonStr);
        }

        /// <summary>
        /// 转为首字母大写
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ToFirstUpperStr(this string str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }

        /// <summary>
        /// 转为首字母小写
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ToFirstLowerStr(this string str)
        {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        /// <summary>
        /// 身份证信息掩盖
        /// </summary>
        /// <param name="value">身份证Id</param>
        /// <returns></returns>
        public static string ToMaskCardId(this string value)
        {
            return ToMaskCardId(value, value.Length - 6, 6);
        }

        /// <summary>
        /// 身份证信息掩盖
        /// </summary>
        /// <param name="value">身份证Id</param>
        /// <param name="maskLength"></param>
        /// <returns></returns>
        public static string ToMaskCardId(this string value, int beginMaskIndex, int maskLength)
        {
            if (value.Length <= maskLength)
            {
                return value;
            }
            else
            {
                var array = value.ToArray();
                for (int i = 0; i < array.Length; i++)
                {
                    if (i >= beginMaskIndex && i < (beginMaskIndex + maskLength))
                    {
                        array[i] = '*';
                    }
                }
                return string.Join("", array);
            }
        }

        /// <summary>
        /// 身份证号校验
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ValidCardId(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            long n = 0;
            if (long.TryParse(value.Remove(17), out n) == false
                || n < Math.Pow(10, 16) || long.TryParse(value.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(value.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = value.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = value.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != value.Substring(17, 1).ToLower())
            {
                return false;//校验码验证  
            }
            return true;//符合GB11643-1999标准
        }
    }
}
