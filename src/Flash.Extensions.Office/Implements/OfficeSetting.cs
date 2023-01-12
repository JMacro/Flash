namespace Flash.Extensions.Office
{
    public class OfficeSetting : IOfficeSetting
    {
        /// <summary>
        /// 默认的Excel设置
        /// </summary>
        public ExcelSetting DefaultExcelSetting { get; private set; } = new ExcelSetting();

        public IOfficeSetting WithDefaultExcelSetting(ExcelSetting setting)
        {
            this.DefaultExcelSetting = setting;
            return this;
        }
    }
}
