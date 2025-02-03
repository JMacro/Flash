using System.ComponentModel.DataAnnotations.Schema;
using Flash.Extensions.Cache;
using Flash.Extensions.Job;
using Flash.Extensions.Job.Hangfire;
using Flash.Extensions.Office;
using Flash.Extensions.ORM;
using Flash.Extensions.UidGenerator;
using Flash.Widgets.DbContexts;
using Hangfire.Server;

namespace Flash.Widgets.Jobs
{
    public class TaoBaoSellItemImportJob : BaseHangfireJob
    {
        private readonly IUniqueIdGenerator _uniqueIdGenerator;
        private readonly TaoBaoDbContext _dbContext;
        private readonly IOfficeTools _officeTools;
        private readonly ICacheManager _cache;
        private string PATH_TaobaoExport = "TaobaoExport";

        public TaoBaoSellItemImportJob(
            ILogger<TaoBaoSellItemImportJob> logger,
            IUniqueIdGenerator uniqueIdGenerator,
            TaoBaoDbContext dbContext,
            IOfficeTools officeTools,
            ICacheManager cache) : base(logger)
        {
            this._uniqueIdGenerator = uniqueIdGenerator;
            this._dbContext = dbContext;
            this._officeTools = officeTools;
            this._cache = cache;
        }

        public override async Task Execute(IJobExecutionContextContainer<PerformContext> contextContainer)
        {
#if DEBUG
            var fileIds = new List<string>();
            fileIds.Add("ExportOrderList20376666815");
#else
            var currData = DateTime.Now.Date;

            var fileIds = this._cache.HashGet<List<string>>("TaoBaoSellItemImport", currData.ToString("yyyy-MM-dd"));
            if (fileIds == null || !fileIds.Any()) return;
#endif

            var queryable = this._dbContext.Set<SellItemOrderEntity>().AsQueryable().Where(p => !p.IsDelete);

            foreach (var fileId in fileIds)
            {
                var excelPath = Path.Combine(Directory.GetCurrentDirectory(), PATH_TaobaoExport, $"{fileId}.xlsx");
                var file = System.IO.File.ReadAllBytes(excelPath);
                var excelList = this._officeTools.ReadExcel<SellItemOrderEntity>(file,"export", SellItemOrderEntity.GetHeaderColumns());
                foreach (var excel in excelList)
                {
                    var sellItem = queryable.FirstOrDefault(p => p.SubOrderId == excel.SubOrderId);
                    if (sellItem == null)
                    {
                        excel.Id = this._uniqueIdGenerator.NewId();
                        excel.CreateTime = DateTime.Now;
                        excel.CreateUserId = 0;
                        excel.LastModifyTime = DateTime.Now;
                        excel.LastModifyUserId = 0;
                        this._dbContext.Add(excel);
                    }
                    else
                    {
                        if (sellItem.OrderStatus == "交易成功" || sellItem.OrderStatus == "交易关闭") continue;

                        sellItem.LastModifyTime = DateTime.Now;
                        sellItem.LastModifyUserId = 0;

                        sellItem.SubOrderId = excel.SubOrderId;
                        sellItem.MainOrderId = excel.MainOrderId;
                        sellItem.PurchaseQuantity = excel.PurchaseQuantity;
                        sellItem.ProductAttributes = excel.ProductAttributes;
                        sellItem.ContactRemarks = excel.ContactRemarks;
                        sellItem.OrderStatus = excel.OrderStatus;
                        sellItem.MerchantCode = excel.MerchantCode;
                        sellItem.BuyerAmountDue = excel.BuyerAmountDue;
                        sellItem.BuyerAmountPaid = excel.BuyerAmountPaid;
                        sellItem.RefundStatus = excel.RefundStatus;
                        sellItem.RefundAmount = excel.RefundAmount;
                        sellItem.OrderCreationTime = excel.OrderCreationTime;
                        sellItem.OrderPaymentTime = excel.OrderPaymentTime;
                        sellItem.RemarkLabel = excel.RemarkLabel;
                        sellItem.MerchantRemarks = excel.MerchantRemarks;
                        sellItem.ShippingTime = excel.ShippingTime;
                        sellItem.TrackingNumber = excel.TrackingNumber;
                        sellItem.LogisticsCompany = excel.LogisticsCompany;
                        this._dbContext.Update(sellItem);
                    }
                }
                this._dbContext.SaveChanges();
            }
        }
    }
}

/// <summary>
/// 子订单实体类
/// </summary>
[Table("taobao_sell_item_order")]
public class SellItemOrderEntity : IEntity<long>
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    public bool IsDelete { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 创建用户 ID
    /// </summary>
    public long CreateUserId { get; set; }

    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime LastModifyTime { get; set; }

    /// <summary>
    /// 最后修改用户 ID
    /// </summary>
    public long LastModifyUserId { get; set; }

    /// <summary>
    /// 子订单编号
    /// </summary>
    public string SubOrderId { get; set; }

    /// <summary>
    /// 主订单编号
    /// </summary>
    public string MainOrderId { get; set; }

    /// <summary>
    /// 购买数量
    /// </summary>
    public int PurchaseQuantity { get; set; }

    /// <summary>
    /// 商品属性
    /// </summary>
    public string ProductAttributes { get; set; }

    /// <summary>
    /// 联系方式备注
    /// </summary>
    public string? ContactRemarks { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public string OrderStatus { get; set; }

    /// <summary>
    /// 商家编码
    /// </summary>
    public string MerchantCode { get; set; }

    /// <summary>
    /// 买家应付货款
    /// </summary>
    public decimal BuyerAmountDue { get; set; }

    /// <summary>
    /// 买家实付金额
    /// </summary>
    public decimal BuyerAmountPaid { get; set; }

    /// <summary>
    /// 退款状态
    /// </summary>
    public string RefundStatus { get; set; }

    /// <summary>
    /// 退款金额
    /// </summary>
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// 订单创建时间
    /// </summary>
    public DateTime OrderCreationTime { get; set; }

    /// <summary>
    /// 订单付款时间
    /// </summary>
    public DateTime OrderPaymentTime { get; set; }

    /// <summary>
    /// 备注标签
    /// </summary>
    public string? RemarkLabel { get; set; }

    /// <summary>
    /// 商家备注
    /// </summary>
    public string? MerchantRemarks { get; set; }

    /// <summary>
    /// 发货时间
    /// </summary>
    public DateTime ShippingTime { get; set; }

    /// <summary>
    /// 物流单号
    /// </summary>
    public string? TrackingNumber { get; set; }

    /// <summary>
    /// 物流公司
    /// </summary>
    public string? LogisticsCompany { get; set; }

    public static List<ExcelHeaderColumn> GetHeaderColumns()
    {
        var printLoggerHeaderColumns = new List<ExcelHeaderColumn>();
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("子订单编号", nameof(SellItemOrderEntity.SubOrderId)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("主订单编号", nameof(SellItemOrderEntity.MainOrderId)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("购买数量", nameof(SellItemOrderEntity.PurchaseQuantity)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("商品属性", nameof(SellItemOrderEntity.ProductAttributes)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("联系方式备注", nameof(SellItemOrderEntity.ContactRemarks)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("订单状态", nameof(SellItemOrderEntity.OrderStatus)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("商家编码", nameof(SellItemOrderEntity.MerchantCode)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("买家应付货款", nameof(SellItemOrderEntity.BuyerAmountDue)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("买家实付金额", nameof(SellItemOrderEntity.BuyerAmountPaid)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("退款状态", nameof(SellItemOrderEntity.RefundStatus)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("退款金额", nameof(SellItemOrderEntity.RefundAmount)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("订单创建时间", nameof(SellItemOrderEntity.OrderCreationTime)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("订单付款时间", nameof(SellItemOrderEntity.OrderPaymentTime)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("备注标签", nameof(SellItemOrderEntity.RemarkLabel)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("商家备注", nameof(SellItemOrderEntity.MerchantRemarks)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("发货时间", nameof(SellItemOrderEntity.ShippingTime)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("物流单号", nameof(SellItemOrderEntity.TrackingNumber)));
        printLoggerHeaderColumns.Add(ExcelHeaderColumn.Create("物流公司", nameof(SellItemOrderEntity.LogisticsCompany)));

        return printLoggerHeaderColumns;
    }
}
