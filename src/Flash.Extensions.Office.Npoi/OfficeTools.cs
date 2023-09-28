using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Extensions;
using Flash.Core;

namespace Flash.Extensions.Office.Npoi
{
    public class OfficeTools : IOfficeTools
    {
        private readonly IOfficeSetting _officeSetting;

        public OfficeTools(IOfficeSetting officeSetting)
        {
            this._officeSetting = officeSetting;
        }

        public List<T> ReadExcel<T>(byte[] bytes, List<ExcelHeaderColumn> columnMaps) where T : new()
        {
            var entitys = new List<T>();
            var dataType = typeof(T);
            var properties = EntityTypeCaches.TryGetOrAddByProperties(dataType);

            using (var memoryStream = new MemoryStream(bytes))
            {
                var columnMapsDic = columnMaps.ToDictionary(p => p.ColumnMap.ExcelColumnName, p => p);

                var xssWorkbook = new HSSFWorkbook(memoryStream);
                var sheet = xssWorkbook.GetSheetAt(0);
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;

                var excelColumns = new Dictionary<int, string>();
                for (int j = 0; j < cellCount; j++)
                {
                    var cell = headerRow.GetCell(j);
                    if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;

                    excelColumns.Add(j, cell.ToString());
                }

                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    var entity = new T();
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            var cellValue = row.GetCell(j).ToString();

                            if (!string.IsNullOrWhiteSpace(cellValue))
                            {
                                var tmp = columnMapsDic[excelColumns[j]];
                                var property = dataType.GetProperty(tmp.ColumnMap.EntityFieldName);
                                if (property != null)
                                {
                                    if (TryConvertType(cellValue, property.PropertyType, out dynamic result))
                                    {
                                        property.SetValue(entity, result);
                                    }
                                }
                            }
                        }
                    }
                    entitys.Add(entity);
                }
            }
            return entitys;
        }

        public List<T> ReadExcel<T>(byte[] bytes, string sheetName, List<ExcelHeaderColumn> columnMaps) where T : new()
        {
            var entitys = new List<T>();
            var dataType = typeof(T);
            var properties = EntityTypeCaches.TryGetOrAddByProperties(dataType);

            using (var memoryStream = new MemoryStream(bytes))
            {
                var columnMapsDic = columnMaps.ToDictionary(p => p.ColumnMap.ExcelColumnName, p => p);

                var xssWorkbook = new HSSFWorkbook(memoryStream);
                var sheet = xssWorkbook.GetSheet(sheetName);
                if (sheet == null) return entitys;

                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;

                var excelColumns = new Dictionary<int, string>();
                for (int j = 0; j < cellCount; j++)
                {
                    var cell = headerRow.GetCell(j);
                    if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;

                    excelColumns.Add(j, cell.ToString());
                }

                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    var entity = new T();
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            var cellValue = row.GetCell(j).ToString();

                            if (!string.IsNullOrWhiteSpace(cellValue))
                            {
                                var tmp = columnMapsDic[excelColumns[j]];
                                var property = dataType.GetProperty(tmp.ColumnMap.EntityFieldName);
                                if (property != null)
                                {
                                    if (TryConvertType(cellValue, property.PropertyType, out dynamic result))
                                    {
                                        property.SetValue(entity, result);
                                    }
                                }
                            }
                        }
                    }
                    entitys.Add(entity);
                }
            }
            return entitys;
        }

        public byte[] WriteExcel<T>(List<T> dataSource, List<ExcelHeaderColumn> headerColumns, SheetSetting setting = null) where T : new()
        {
            return WriteExcelMultipleSheet(SheetInfo.Create<T>(dataSource, headerColumns, setting));
        }

        public byte[] WriteExcelMultipleSheet(params SheetInfo[] sheets)
        {
            using (var memoryStream = new MemoryStream())
            {
                HSSFWorkbook workbook = new HSSFWorkbook();

                ICellStyle style = workbook.CreateCellStyle();
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                style.VerticalAlignment = VerticalAlignment.Center;

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.CloneStyleFrom(style);
                styleHeader.FillForegroundColor = 30;
                styleHeader.FillPattern = FillPattern.SolidForeground;
                styleHeader.Alignment = HorizontalAlignment.Center;
                styleHeader.VerticalAlignment = VerticalAlignment.Center;

                IFont fontHeader = workbook.CreateFont();
                fontHeader.IsBold = true;
                fontHeader.FontName = "微软雅黑";
                fontHeader.Color = HSSFColor.White.Index;
                styleHeader.SetFont(fontHeader);

                Dictionary<string, ICellStyle> cellStyleDic = new Dictionary<string, ICellStyle>();
                foreach (var sheetInfo in sheets)
                {
                    var columnIndex = 0;
                    var rowIndex = 0;

                    if (workbook.GetSheet(sheetInfo.SheetName) != null)
                    {
                        throw new ArgumentException($"Sheet name repeat[{sheetInfo.SheetName}]");
                    }

                    ISheet excelSheet = workbook.CreateSheet(sheetInfo.SheetName);
                    excelSheet.DefaultColumnWidth = 100 * 256;
                    excelSheet.DefaultRowHeight = 16 * 20;
                    excelSheet.DisplayGridlines = sheetInfo.SheetSetting?.DisplayGridlines ?? true;
                    excelSheet.IsPrintGridlines = sheetInfo.SheetSetting?.IsPrintGridlines ?? false;

                    // 创建绘图主控制器(用于包括单元格注释在内的所有形状的顶级容器)
                    var patriarch = excelSheet.CreateDrawingPatriarch();

                    IRow row = excelSheet.CreateRow(rowIndex++);

                    if (sheetInfo.SheetSetting?.HeaderRowHeight > 0) row.Height = Convert.ToInt16(sheetInfo.SheetSetting.HeaderRowHeight * 20);
                    else row.Height = 16 * 20;

                    if (sheetInfo.SheetSetting.IsAutoNumber)
                    {
                        ICell cell = row.CreateCell(columnIndex);
                        cell.CellStyle = styleHeader;
                        cell.SetCellValue(string.IsNullOrWhiteSpace(sheetInfo.SheetSetting.AutoNumberName) ? "序号" : sheetInfo.SheetSetting.AutoNumberName);
                        excelSheet.SetColumnWidth(columnIndex, 8 * 256);
                        columnIndex++;
                    }

                    foreach (var column in sheetInfo.HeaderColumns)
                    {
                        ICell cell = row.CreateCell(columnIndex);
                        cell.CellStyle = styleHeader;
                        cell.SetCellValue(column.ColumnMap.ExcelColumnName);
                        cell.CreateComment(column.Comment);

                        if (column.ColumnWidth > 0)
                        {
                            excelSheet.SetColumnWidth(columnIndex, column.ColumnWidth * 256);
                        }
                        else
                        {
                            var columnWidth = Encoding.Default.GetBytes(column.ColumnMap.ExcelColumnName).Length;
                            excelSheet.SetColumnWidth(columnIndex, columnWidth * 256);
                        }
                        columnIndex++;
                    }

                    var dataType = sheetInfo.DataSourceType;
                    var properties = EntityTypeCaches.TryGetOrAddByProperties(dataType);

                    var dataformat = workbook.CreateDataFormat();

                    var dataIndex = 1;
                    //写入数据
                    foreach (var data in sheetInfo.DataSource)
                    {
                        var dataRow = excelSheet.CreateRow(rowIndex++);
                        if (sheetInfo.SheetSetting?.DataRowHeight > 0) dataRow.Height = Convert.ToInt16(sheetInfo.SheetSetting.DataRowHeight * 20);
                        else dataRow.Height = 16 * 20;

                        columnIndex = 0;
                        if (sheetInfo.SheetSetting.IsAutoNumber)
                        {
                            var cell = dataRow.CreateCell(columnIndex);
                            cell.CellStyle = style;
                            cell.SetCellValue(dataIndex);
                            columnIndex++;
                            dataIndex++;
                        }

                        foreach (var column in sheetInfo.HeaderColumns)
                        {
                            var cell = dataRow.CreateCell(columnIndex);
                            cell.CellStyle = style;
                            columnIndex++;

                            var propertieInfo = properties.FirstOrDefault(p => p.Name == column.ColumnMap.EntityFieldName);
                            if (propertieInfo != null)
                            {
                                NopiExtensions.SetCellValue(cell, propertieInfo.GetValue(data));
                                if (!string.IsNullOrEmpty(column.DataFormat))
                                {
                                    if (!cellStyleDic.TryGetValue(column.DataFormat, out var dataStyle))
                                    {
                                        dataStyle = workbook.CreateCellStyle();
                                        dataStyle.CloneStyleFrom(style);
                                        cellStyleDic.Add(column.DataFormat, dataStyle);
                                    }
                                    dataStyle.DataFormat = dataformat.GetFormat(column.DataFormat);
                                    cell.CellStyle = dataStyle;
                                }
                            }
                        }
                    }
                }
                workbook.Write(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private bool TryConvertType(object value, Type targetType, out dynamic result)
        {
            result = null;
            if (value == null) return false;

            var stringValue = value as string;

            if (!string.IsNullOrEmpty(stringValue))
            {
                if (targetType == typeof(string))
                {
                    result = stringValue;
                    return true;
                }

                if (targetType == typeof(DateTime))
                {
                    if (DateTime.TryParse(stringValue, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime dateTime))
                    {
                        result = dateTime;
                        return true;
                    }
                }

                if (targetType.IsNumeric() && double.TryParse(stringValue, NumberStyles.Any, null, out double doubleResult))
                {
                    if (targetType.FullName.StartsWith("System.Nullable`1[[System.Decimal"))
                    {
                        targetType = typeof(decimal);
                    }

                    if (targetType.FullName.StartsWith("System.Nullable`1[[System.Int64"))
                    {
                        targetType = typeof(long);
                    }
                    else if (targetType.FullName.StartsWith("System.Nullable`1[[System.Int"))
                    {
                        targetType = typeof(int);
                    }

                    if (targetType.FullName.StartsWith("System.Nullable`1[[System.Single"))
                    {
                        targetType = typeof(float);
                    }


                    result = Convert.ChangeType(doubleResult, targetType);
                    return true;
                }
            }

            if (targetType.IsEnum)
            {
                result = Enum.Parse(targetType, stringValue, true);
                return true;
            }

            if (targetType == typeof(Guid))
            {
                var parsed = Guid.TryParse(stringValue, out var guidResult);
                result = guidResult;
                return parsed;
            }

            try
            {
                result = Convert.ChangeType(value, targetType);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

    public static class NopiExtensions
    {
        public static void SetCellValue(this ICell cell, object value)
        {
            if (value == null)
            {
                cell.SetCellValue("");
                return;
            }

            var targetType = value.GetType();

            var stringValue = Convert.ToString(value);
            if (targetType == typeof(string) || targetType.IsEnum || targetType == typeof(Guid))
            {
                cell.SetCellValue(stringValue);
                return;
            }

            if (targetType == typeof(DateTime))
            {
                if (DateTime.TryParse(stringValue, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime dateTime))
                {
                    cell.SetCellValue(dateTime);
                    return;
                }
            }

            if (targetType.IsNumeric() && double.TryParse(stringValue, NumberStyles.Any, null, out double doubleResult))
            {
                cell.SetCellValue(doubleResult);
                return;
            }

            if (targetType == typeof(Boolean) && Boolean.TryParse(stringValue, out bool boolResult))
            {
                cell.SetCellValue(boolResult);
                return;
            }

            cell.SetCellValue("");
        }

        /// <summary>
        /// 创建Npoi批注
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <param name="excelComment">批注</param>
        public static void CreateComment(this ICell cell, IExcelComment excelComment)
        {
            // 创建绘图主控制器(用于包括单元格注释在内的所有形状的顶级容器)
            var patriarch = cell.Sheet.CreateDrawingPatriarch();

            if (excelComment != null)
            {
                var comment = patriarch.CreateCellComment(new HSSFClientAnchor());
                if (string.IsNullOrEmpty(excelComment.Author)) excelComment.Author = "System";
                comment.Author = excelComment.Author;
                comment.String = new HSSFRichTextString($"{excelComment.Author}:{Environment.NewLine}{excelComment.Content ?? ""}");
                comment.Visible = excelComment.DefaultVisible;
                cell.CellComment = comment;
            }
        }
    }
}
