using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.ChangeHistory
{
    /// <summary>
    /// 比较结果
    /// </summary>
    public class CompareResult
    {
        /// <summary>
        /// 是否变更
        /// </summary>
        public bool IsChange { get; set; }

        /// <summary>
        /// 变更内容
        /// </summary>
        public ChangeHistoryInfo ChangeHistories { get; set; }
    }
}
