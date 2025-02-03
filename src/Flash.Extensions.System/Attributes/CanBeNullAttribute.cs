
using System;

namespace Flash.Extensions
{
    [AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Parameter |
    AttributeTargets.Property | AttributeTargets.Delegate |
    AttributeTargets.Field)]
    public sealed class CanBeNullAttribute : Attribute { }
}
