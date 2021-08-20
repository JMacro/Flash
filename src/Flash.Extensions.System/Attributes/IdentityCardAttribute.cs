#if NETCORE
namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// 身份证验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public sealed class IdentityCardAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            string valueAsString = value as string;
            return valueAsString != null && valueAsString.ValidCardId();
        }
    }
}
#endif

#if NET45

#endif