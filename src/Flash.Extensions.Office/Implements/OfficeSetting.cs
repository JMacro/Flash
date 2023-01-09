using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.Office
{
    public class OfficeSetting : IOfficeSetting
    {
        public ExcelSetting DefaultExcelSetting { get; private set; }

        public IOfficeSetting WithDefaultExcelSetting(ExcelSetting setting)
        {
            this.DefaultExcelSetting = setting;
            return this;
        }
    }
}
