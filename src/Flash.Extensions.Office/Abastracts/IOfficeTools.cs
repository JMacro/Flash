using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extensions.Office
{
    public interface IOfficeTools
    {
        /// <summary>
        /// 读取Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="columnMaps"></param>
        /// <returns></returns>
        List<T> ReadExcel<T>(byte[] bytes, List<ExcelHeaderColumn> columnMaps) where T : new();

        /// <summary>
        /// 写入Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource">数据源</param>
        /// <param name="headerColumns">表头列</param>
        /// <param name="setting">属性设置</param>
        /// <returns></returns>
        byte[] WriteExcel<T>(List<T> dataSource, List<ExcelHeaderColumn> headerColumns, ExcelSetting setting = null) where T : new();
    }
}
