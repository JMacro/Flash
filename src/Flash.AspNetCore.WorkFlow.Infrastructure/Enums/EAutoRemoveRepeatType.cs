using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.Enums
{
    /// <summary>
    /// 自动去重
    /// </summary>
    public enum EAutoRemoveRepeatType
    {
        /// <summary>
        /// 只保留第一个
        /// </summary>
        OnlyRetainOne = 1,
        /// <summary>
        /// 仅连续存在时，去重
        /// </summary>
        OnlyContinuityExis = 2
    }
}
