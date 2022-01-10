using System;

namespace Flash.Extensions
{
    /// <summary>
    /// 实体映射标签
    /// </summary>
    public class AutoMapperToAttribute : Attribute
    {
        private readonly Type[] _types;

        /// <summary>
        /// 实体映射
        /// </summary>
        /// <param name="types">需映射的类型</param>
        public AutoMapperToAttribute(params Type[] types)
        {
            this._types = types;
        }

        /// <summary>
        /// 获得映射的类型
        /// </summary>
        /// <returns></returns>
        public Type[] GetMapperList()
        {
            return this._types;
        }
    }
}
