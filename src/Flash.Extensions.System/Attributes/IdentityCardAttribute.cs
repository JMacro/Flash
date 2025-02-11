#if NETCORE
using System;
using System.ComponentModel.DataAnnotations;

namespace Flash.Extensions
{
    /// <summary>
    /// 身份证验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public sealed class IdentityCardAttribute : ValidationAttribute
    {
        public IdentityCardAttribute()
        {
            this.ErrorMessage = "身份证号验证不通过";
        }

        public IdentityCardAttribute(string errorMessage) : base(errorMessage)
        {
        }

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