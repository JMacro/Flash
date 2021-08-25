namespace System
{
    /// <summary>
    /// 数据类型标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public class DataTypeAttribute : Attribute
    {
        private readonly Type _type;

        /// <summary>
        /// 数据类型标记
        /// </summary>
        /// <param name="type">需标记的类型</param>
        public DataTypeAttribute(Type type)
        {
            this._type = type;
        }

        /// <summary>
        /// 获得标记的类型
        /// </summary>
        /// <returns></returns>
        public Type GetDataType()
        {
            return this._type;
        }
    }
}
