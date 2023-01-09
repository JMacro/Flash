
using Flash.Extensions.Office;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Flash.Test
{
    [TestClass]
    public class OfficeTest : BaseTest
    {
        [TestMethod]
        public void TestWriteExcel()
        {
            var tool = ServiceProvider.GetService<IOfficeTools>();

            var headerColumns = new List<ExcelHeaderColumn>();
            headerColumns.Add(ExcelHeaderColumn.Create("序号", nameof(StudentInfo.Id)));
            headerColumns.Add(ExcelHeaderColumn.Create("姓名", nameof(StudentInfo.Name)));
            headerColumns.Add(ExcelHeaderColumn.Create("年龄", nameof(StudentInfo.Age)));
            headerColumns.Add(ExcelHeaderColumn.Create("平均收入", nameof(StudentInfo.Monery), "[Green]0.00;[Red]-0.00"));
            headerColumns.Add(ExcelHeaderColumn.Create("日期", nameof(StudentInfo.Date), "yyyy-MM-dd"));
            headerColumns.Add(ExcelHeaderColumn.Create("枚举", nameof(StudentInfo.Enum)));
            headerColumns.Add(ExcelHeaderColumn.Create("是否启用", nameof(StudentInfo.IsEnable)));

            var dataSource = new List<StudentInfo>();
            var randon = new Random((int)TimeSpan.TicksPerSecond);
            for (int i = 1; i <= 1000; i++)
            {
                dataSource.Add(new StudentInfo { Id = Guid.NewGuid(), Name = $"姓名{i}", Age = 11, Date = DateTime.Now, Monery = 100000 - randon.NextDouble() * 1000000, Enum = TestEnum.TT, IsEnable = false });
            }

            var buffer = tool.WriteExcel(dataSource, headerColumns);
            Assert.IsNotNull(buffer);

            File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "excel", DateTime.Now.ToFileTime().ToString() + ".xls"), buffer);


            var datas = tool.ReadExcel<StudentInfo>(buffer, headerColumns);
            Assert.IsNotNull(datas);
        }
    }

    public class StudentInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }
        public double Monery { get; set; }
        public DateTime Date { get; set; }
        public TestEnum Enum { get; set; }
        public bool IsEnable { get; set; }
    }

    public enum TestEnum
    {
        All,
        TT
    }
}
