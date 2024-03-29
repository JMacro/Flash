﻿using System.Collections.Generic;

namespace Flash.Extensions.Office
{
    public interface IOfficeTools
    {
        /// <summary>
        /// 读取Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="columnMaps">表头列</param>
        /// <returns></returns>
        List<T> ReadExcel<T>(byte[] bytes, List<ExcelHeaderColumn> columnMaps) where T : new();

        /// <summary>
        /// 读取Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="sheetName">Sheet名称</param>
        /// <param name="columnMaps">表头列</param>
        /// <returns></returns>
        List<T> ReadExcel<T>(byte[] bytes, string sheetName, List<ExcelHeaderColumn> columnMaps) where T : new();

        /// <summary>
        /// 写入Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource">数据源</param>
        /// <param name="headerColumns">表头列</param>
        /// <param name="setting">属性设置</param>
        /// <returns></returns>
        byte[] WriteExcel<T>(List<T> dataSource, List<ExcelHeaderColumn> headerColumns, SheetSetting setting = null) where T : new();

        /// <summary>
        /// 写入Excel（多个Sheet数据），可通过<see cref="SheetInfo.Create"/>创建对象
        /// </summary>
        /// <param name="sheets">Sheet信息</param>
        /// <returns></returns>
        byte[] WriteExcelMultipleSheet(params SheetInfo[] sheets);
    }
}
