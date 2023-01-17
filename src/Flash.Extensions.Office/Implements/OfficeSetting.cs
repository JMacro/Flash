namespace Flash.Extensions.Office
{
    public class OfficeSetting : IOfficeSetting
    {
        /// <summary>
        /// 默认的Excel设置
        /// </summary>
        public SheetSetting DefaultExcelSetting { get; private set; } = new SheetSetting();

        public IOfficeSetting WithDefaultExcelSetting(SheetSetting setting)
        {
            this.DefaultExcelSetting = setting;
            return this;
        }
    }
}
