using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flash.Extensions.Office;
using Flash.Test.StartupTests;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Flash.Test.Offices
{
    [TestFixture]
    public class OfficeTests : BaseTest<OfficeStartupTest>
    {
        [Test]
        public void WriteExcelTest()
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

            var datas = tool.ReadExcel<StudentInfo>(buffer, headerColumns);
            Assert.IsNotNull(datas);
        }

        [Test]
        public void WriteExcelByMultipleSheetTest()
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

            var datas = tool.ReadExcel<StudentInfo>(buffer, "Sheet2", headerColumns2);
            Assert.IsNotNull(datas);
        }

        [Test]
        public void ReadExcelByExportOrderInfo()
        {
            var tool = ServiceProvider.GetService<IOfficeTools>();
            var headerColumns = new List<ExcelHeaderColumn>();
            headerColumns.Add(ExcelHeaderColumn.Create("主订单编号", nameof(ExportOrderInfo.MainOrderNumber)));
            headerColumns.Add(ExcelHeaderColumn.Create("购买数量", nameof(ExportOrderInfo.BuyNumber)));
            headerColumns.Add(ExcelHeaderColumn.Create("订单状态", nameof(ExportOrderInfo.OrderStatus)));
            headerColumns.Add(ExcelHeaderColumn.Create("商家编码", nameof(ExportOrderInfo.ShopCode)));
            headerColumns.Add(ExcelHeaderColumn.Create("买家实际支付金额", nameof(ExportOrderInfo.BuyActualAmount)));
            headerColumns.Add(ExcelHeaderColumn.Create("退款状态", nameof(ExportOrderInfo.RefundStatus)));
            headerColumns.Add(ExcelHeaderColumn.Create("订单创建时间", nameof(ExportOrderInfo.OrderCreateTime)));
            headerColumns.Add(ExcelHeaderColumn.Create("订单付款时间", nameof(ExportOrderInfo.OrderPaymentTime)));
            headerColumns.Add(ExcelHeaderColumn.Create("商家备注", nameof(ExportOrderInfo.MerchantRemarks)));
            headerColumns.Add(ExcelHeaderColumn.Create("发货时间", nameof(ExportOrderInfo.DeliveryTime)));

            var excelPath = Path.Combine(Directory.GetCurrentDirectory(), "ExportOrderList18035658052.xlsx");
            var file = File.ReadAllBytes(excelPath);
            var excelList = tool.ReadExcel<ExportOrderInfo>(file, headerColumns);

            var deliveryDate = new DateTime(2024, 1, 1);
            var tmp = excelList.Where(p=> p.DeliveryDate == deliveryDate).ToList();

        }

        [Test]
        public void ReadExcelByOrderPrintLogger()
        {
            var tool = ServiceProvider.GetService<IOfficeTools>();
            var headerColumns = new List<ExcelHeaderColumn>();
            headerColumns.Add(ExcelHeaderColumn.Create("订单编号", nameof(OrderPrintLogger.OrderNumber)));
            headerColumns.Add(ExcelHeaderColumn.Create("打印时间", nameof(OrderPrintLogger.PrintTime)));
            headerColumns.Add(ExcelHeaderColumn.Create("打印类型", nameof(OrderPrintLogger.PrintType)));

            var excelPath = Path.Combine(Directory.GetCurrentDirectory(), "OrderPrintLogger.xlsx");
            var file = File.ReadAllBytes(excelPath);
            var excelList = tool.ReadExcel<OrderPrintLogger>(file, headerColumns);

            var printDate = new DateTime(2024, 1, 1);
            var tmp = excelList
                .Where(p => p.PrintDate == printDate)
                .SelectMany(p => string.IsNullOrWhiteSpace(p.OrderNumber) ? new string[0] : (p.OrderNumber.Split(",\n")))
                .ToList();

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

    public class ExportOrderInfo
    {
        /// <summary>
        /// 主订单编号
        /// </summary>
        public string MainOrderNumber { get; set; }
        /// <summary>
        /// 购买数量
        /// </summary>
        public int BuyNumber { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderStatus { get; set; }
        /// <summary>
        /// 商家编码(商品编码)
        /// </summary>
        public string ShopCode { get; set; }	
        /// <summary>
        /// 买家实际支付金额
        /// </summary>
        public decimal BuyActualAmount { get; set; }
        /// <summary>
        /// 退款状态
        /// </summary>
        public string RefundStatus { get; set; }
        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime OrderCreateTime { get; set; }
        /// <summary>
        /// 订单付款时间
        /// </summary>
        public DateTime OrderPaymentTime { get; set; }
        /// <summary>
        /// 商家备注
        /// </summary>
        public string MerchantRemarks { get; set; }
        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime DeliveryTime { get; set; }
        /// <summary>
        /// 发货日期
        /// </summary>
        public DateTime DeliveryDate => DeliveryTime.Date;
    }

    public class OrderPrintLogger
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime PrintTime { get; set; }
        /// <summary>
        /// 打印日期
        /// </summary>
        public DateTime PrintDate => PrintTime.Date;
        /// <summary>
        /// 打印类型
        /// </summary>
        public string PrintType { get; set; }
    }
}
