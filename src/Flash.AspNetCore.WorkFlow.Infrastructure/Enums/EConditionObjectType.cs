using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.Enums
{
    /// <summary>
    /// 条件对象类型
    /// </summary>
    public enum EConditionObjectType
    {
        /// <summary>
        /// 表达式
        /// </summary>
        Expression = 1,
        /// <summary>
        /// 逻辑运算符
        /// </summary>
        LogicalOperator = 2,
        /// <summary>
        /// 数学运算符
        /// </summary>
        MathOperator = 3,
        /// <summary>
        /// 左括号
        /// </summary>
        LeftBracket = 4,
        /// <summary>
        /// 右括号
        /// </summary>
        RightBracket = 5,
        /// <summary>
        /// 字段
        /// </summary>
        Field = 6,
        /// <summary>
        /// 值
        /// </summary>
        Value = 7
    }
}
