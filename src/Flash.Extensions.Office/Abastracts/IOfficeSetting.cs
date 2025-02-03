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
        SheetSetting DefaultExcelSetting { get; }

        IOfficeSetting WithDefaultExcelSetting(SheetSetting setting);
    }
}
