using System;
using System.Collections.Generic;

namespace Flash.Extensions.Office
{
    public class SheetInfo
    {
        /// <summary>
        /// Sheel名称
        /// </summary>
        public string SheetName { get; private set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public dynamic DataSource { get; private set; }
        /// <summary>
        /// 数据源类型
        /// </summary>
        public Type DataSourceType { get; private set; }
        /// <summary>
        /// 表头
        /// </summary>
        public List<ExcelHeaderColumn> HeaderColumns { get; set; } = new List<ExcelHeaderColumn>();
        /// <summary>
        /// Sheel设置
        /// </summary>
        public ExcelSetting SheetSetting { get; set; } = new ExcelSetting();

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="headerColumns">表头</param>
        /// <returns></returns>
        public static SheetInfo Create<T>(List<T> dataSource, List<ExcelHeaderColumn> headerColumns)
        {
            return Create("Sheet1", dataSource, headerColumns);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="headerColumns">表头</param>
        /// <param name="sheetSetting">Sheel设置</param>
        /// <returns></returns>
        public static SheetInfo Create<T>(List<T> dataSource, List<ExcelHeaderColumn> headerColumns, ExcelSetting sheetSetting)
        {
            return Create("Sheet1", dataSource, headerColumns, sheetSetting);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="sheetName">Sheel名称</param>
        /// <param name="dataSource">数据源</param>
        /// <param name="headerColumns">表头</param>
        /// <returns></returns>
        public static SheetInfo Create<T>(string sheetName, List<T> dataSource, List<ExcelHeaderColumn> headerColumns)
        {
            return Create(sheetName, dataSource, headerColumns, new ExcelSetting());
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="sheetName">Sheel名称</param>
        /// <param name="dataSource">数据源</param>
        /// <param name="headerColumns">表头</param>
        /// <param name="sheetSetting">Sheel设置</param>
        /// <returns></returns>
        public static SheetInfo Create<T>(string sheetName, List<T> dataSource, List<ExcelHeaderColumn> headerColumns, ExcelSetting sheetSetting)
        {
            return new SheetInfo
            {
                SheetName = sheetName,
                DataSource = dataSource,
                DataSourceType = typeof(T),
                HeaderColumns = headerColumns,
                SheetSetting = sheetSetting
            };
        }
    }
}
