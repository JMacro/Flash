using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.Office
{
    public interface IOfficeSetting
    {
        /// <summary>
        /// 默认Excel设置
        /// </summary>
        ExcelSetting DefaultExcelSetting { get; }

        IOfficeSetting WithDefaultExcelSetting(ExcelSetting setting);
    }
}
