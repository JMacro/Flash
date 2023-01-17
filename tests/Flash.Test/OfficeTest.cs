
using Flash.Extensions.Email;
using Flash.Extensions.Office;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

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
            headerColumns.Add(ExcelHeaderColumn.Create("Id", nameof(StudentInfo.Id)));
            headerColumns.Add(ExcelHeaderColumn.Create("姓名", nameof(StudentInfo.Name)));
            headerColumns.Add(ExcelHeaderColumn.Create("年龄", nameof(StudentInfo.Age)));
            headerColumns.Add(ExcelHeaderColumn.Create("平均收入", nameof(StudentInfo.Monery), "[Green]0.00;[Red]-0.00"));
            headerColumns.Add(ExcelHeaderColumn.Create("日期", nameof(StudentInfo.Date), "yyyy-MM-dd", 11));
            headerColumns.Add(ExcelHeaderColumn.Create("枚举", nameof(StudentInfo.Enum)));
            headerColumns.Add(ExcelHeaderColumn.Create("是否启用", nameof(StudentInfo.IsEnable)));
            headerColumns.Add(ExcelHeaderColumn.Create("评分", nameof(StudentInfo.Score), 5, ExcelComment.Create("这是评分批注内容", "作者")));

            var dataSource = new List<StudentInfo>();
            var randon = new Random((int)TimeSpan.TicksPerSecond);
            for (int i = 1; i <= 1000; i++)
            {
                dataSource.Add(new StudentInfo { Id = Guid.NewGuid(), Name = $"姓名{i}", Age = 11, Date = DateTime.Now, Monery = 100000 - randon.NextDouble() * 1000000, Enum = TestEnum.TT, IsEnable = false });
            }

            var buffer = tool.WriteExcel(dataSource, headerColumns, new SheetSetting
            {
                HeaderRowHeight = 46,
                DataRowHeight = 32,
                DisplayGridlines = false,
            });
            Assert.IsNotNull(buffer);

            var filePath = Path.Combine(AppContext.BaseDirectory, "excel");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            File.WriteAllBytes(Path.Combine(filePath, DateTime.Now.ToFileTime().ToString() + ".xls"), buffer);


            var datas = tool.ReadExcel<StudentInfo>(buffer, headerColumns);
            Assert.IsNotNull(datas);
        }

        [TestMethod]
        public void TestWriteExcelByMultipleSheet()
        {
            var tool = ServiceProvider.GetService<IOfficeTools>();
            Assert.IsNotNull(tool);

            var headerColumns1 = ExcelHeaderColumn.Create<StudentInfo>();
            Assert.IsNotNull(headerColumns1);

            var headerColumns2 = new List<ExcelHeaderColumn>();
            headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-Id", nameof(StudentInfo.Id)));
            headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-姓名", nameof(StudentInfo.Name)));
            headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-年龄", nameof(StudentInfo.Age)));
            headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-平均收入", nameof(StudentInfo.Monery), "[Green]0.00;[Red]-0.00"));
            headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-日期", nameof(StudentInfo.Date), "yyyy-MM-dd"));
            headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-枚举", nameof(StudentInfo.Enum)));
            headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-是否启用", nameof(StudentInfo.IsEnable)));
            headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-评分", nameof(StudentInfo.Score), 5, ExcelComment.Create("这是评分批注内容", "作者")));

            var headerColumns3 = new List<ExcelHeaderColumn>();
            headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-Id", nameof(StudentInfo.Id)));
            headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-姓名", nameof(StudentInfo.Name)));
            headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-年龄", nameof(StudentInfo.Age)));
            headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-平均收入", nameof(StudentInfo.Monery), "[Green]0.00;[Red]-0.00"));
            headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-日期", nameof(StudentInfo.Date), "yyyy-MM-dd"));
            headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-枚举", nameof(StudentInfo.Enum)));
            headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-是否启用", nameof(StudentInfo.IsEnable)));
            headerColumns3.Add(ExcelHeaderColumn.Create("Sheet3-评分", nameof(StudentInfo.Score), 5, ExcelComment.Create("这是评分批注内容", "作者")));

            var dataSource = new List<StudentInfo>();
            var randon = new Random((int)TimeSpan.TicksPerSecond);
            for (int i = 1; i <= 10000; i++)
            {
                dataSource.Add(new StudentInfo { Id = Guid.NewGuid(), Name = $"姓名{i}", Age = 11, Date = DateTime.Now, Monery = 100000 - randon.NextDouble() * 1000000, Enum = TestEnum.TT, IsEnable = false });
            }

            var buffer = tool.WriteExcelMultipleSheet(
                SheetInfo.Create("Sheet1", dataSource, headerColumns1, new SheetSetting
                {
                    IsAutoNumber = true,
                    HeaderRowHeight = 46,
                    DataRowHeight = 32,
                    DisplayGridlines = false
                }),
                SheetInfo.Create("Sheet2", dataSource, headerColumns2),
                SheetInfo.Create("Sheet3", dataSource, headerColumns3));
            Assert.IsNotNull(buffer);

            var fileName = DateTime.Now.ToFileTime().ToString() + ".xls";
            var filePath = Path.Combine(AppContext.BaseDirectory, "excel");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            File.WriteAllBytes(Path.Combine(filePath, fileName), buffer);

            var datas = tool.ReadExcel<StudentInfo>(buffer, "Sheet2", headerColumns2);
            Assert.IsNotNull(datas);

            var emailService = ServiceProvider.GetService<IEmailService>();
            Assert.IsNotNull(emailService);

            //emailService.Send("XXXX@163.com", "邮箱发送测试", "邮箱发送测试", AttachmentInfo.Create(fileName, new MemoryStream(buffer)), System.Text.Encoding.UTF8);
            //emailService.Send("XXXX@163.com", "邮箱发送测试", "邮箱发送测试Path", AttachmentInfo.Create(Path.Combine(AppContext.BaseDirectory, "excel", fileName)), System.Text.Encoding.UTF8);
            //emailService.Send("XXXX@163.com", "邮箱发送测试", "邮箱发送测试Path", AttachmentInfo.Create(Path.Combine(AppContext.BaseDirectory, "excel", fileName), Path.Combine(AppContext.BaseDirectory, "excel", fileName)), System.Text.Encoding.UTF8);
        }
    }

    public class StudentInfo
    {
        [ExcelHeader("", "Id", 39)]
        public Guid Id { get; set; }
        [ExcelHeader(nameof(Id), "姓名")]
        public string Name { get; set; }
        [ExcelHeader("", "年龄")]
        public int Age { get; set; }
        [ExcelHeader(nameof(Age), "平均收入", "[Green]0.00;[Red]-0.00")]
        public double Monery { get; set; }
        [ExcelHeader(nameof(Monery), "日期", "yyyy-MM-dd", 11)]
        public DateTime Date { get; set; }
        [ExcelHeader(nameof(Date), "枚举")]
        public TestEnum Enum { get; set; }
        [ExcelHeader(nameof(Enum), "是否启用")]
        public bool IsEnable { get; set; }
        [ExcelHeader(nameof(IsEnable), "评分", 5, "这是评分批注内容", "作者")]
        public double Score { get; set; } = 5;
    }

    public enum TestEnum
    {
        All,
        TT
    }
}
