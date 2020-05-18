using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json
{
    public static class JsonExtersion
    {
        /// <summary>
        /// 序列化Json（安全方式）
        /// </summary>
        /// <typeparam name="T">反序列化对象类型</typeparam>
        /// <param name="this">待反序列化串</param>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        public static T DeserializeObjectSafe<T>(this string @this,T defValue = default(T))
        {
            var json = @this.ToString();

            if (typeof(T).IsValueType || typeof(T).FullName == "System.String")
            {
                return (T)Convert.ChangeType(json, typeof(T));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    return defValue;
                }

                try
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (Exception e)
                {
                    return defValue;
                }
            }
        }
    }
}
