namespace System
{
    /// <summary>
    /// 标记不为null
    /// </summary>
    [AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Parameter |
    AttributeTargets.Property | AttributeTargets.Delegate |
    AttributeTargets.Field)]
    public sealed class NotNullAttribute : Attribute { }
}
