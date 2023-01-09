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

namespace Flash.Extensions.Office.Npoi
{
    public class OfficeTools : IOfficeTools
    {
        public List<T> ReadExcel<T>(byte[] bytes, List<ExcelHeaderColumn> columnMaps) where T : new()
        {
            var entitys = new List<T>();
            var dataType = typeof(T);
            var properties = dataType.GetProperties();

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

        public byte[] WriteExcel<T>(List<T> dataSource, List<ExcelHeaderColumn> headerColumns, ExcelSetting setting = null) where T : new()
        {
            var columnIndex = 0;
            var rowIndex = 0;

            using (var memoryStream = new MemoryStream())
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Sheet1");
                excelSheet.DefaultColumnWidth = 100 * 256;
                excelSheet.DefaultRowHeight = 16 * 20;

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

                IRow row = excelSheet.CreateRow(rowIndex++);

                if (setting?.HeaderRowHeight > 0) row.Height = setting.HeaderRowHeight;
                else row.Height = 16 * 20;

                foreach (var column in headerColumns)
                {
                    ICell cell = row.CreateCell(columnIndex);
                    cell.CellStyle = styleHeader;
                    cell.SetCellValue(column.ColumnMap.ExcelColumnName);
                    if (setting?.ColumnWidth > 0)
                    {
                        excelSheet.SetColumnWidth(columnIndex, setting.ColumnWidth * 256);
                    }
                    else
                    {
                        var columnWidth = Encoding.Default.GetBytes(column.ColumnMap.ExcelColumnName).Length;
                        excelSheet.SetColumnWidth(columnIndex, columnWidth * 256);
                    }

                    columnIndex++;
                }

                var dataType = typeof(T);
                var properties = dataType.GetProperties();

                var dataformat = workbook.CreateDataFormat();

                //写入数据
                foreach (var data in dataSource)
                {
                    row = excelSheet.CreateRow(rowIndex++);
                    if (setting?.HeaderRowHeight > 0) row.Height = setting.RowHeight;
                    else row.Height = 16 * 20;

                    columnIndex = 0;

                    foreach (var column in headerColumns)
                    {
                        var cell = row.CreateCell(columnIndex);
                        cell.CellStyle = style;
                        columnIndex++;

                        var propertieInfo = properties.FirstOrDefault(p => p.Name == column.ColumnMap.EntityFieldName);
                        if (propertieInfo != null)
                        {
                            cell.SetCellValue(propertieInfo.GetValue(data));
                            if (!string.IsNullOrEmpty(column.DataFormat))
                            {
                                ICellStyle dataStyle = workbook.CreateCellStyle();
                                dataStyle.CloneStyleFrom(style);
                                dataStyle.DataFormat = dataformat.GetFormat(column.DataFormat);
                                cell.CellStyle = dataStyle;
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
            if (value == null) cell.SetCellValue("");

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
    }
}
