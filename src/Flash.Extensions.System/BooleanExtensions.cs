using System;

namespace Flash.Extensions
{
    public static class BooleanExtensions
    {
        public static Int32 ToInt32(this bool value)
        {
            return value ? 0 : 1;
        }
    }
}
