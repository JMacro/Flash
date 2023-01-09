using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.Office
{
    /// <summary>
    /// Excel属性设置
    /// </summary>
    public class ExcelSetting
    {
        /// <summary>
        /// 列宽
        /// </summary>
        public int ColumnWidth { get; set; }
        /// <summary>
        /// 行高
        /// </summary>
        public short RowHeight { get; set; }
        /// <summary>
        /// 标题头行高
        /// </summary>
        public short HeaderRowHeight { get; set; }
    }
}
