using System.Security.Cryptography;
using System.Text;
using Flash.Extensions;
using Flash.Extensions.Cache;
using Flash.Extensions.Office;
using Flash.Extensions.ORM;
using Flash.Extensions.Resilience.Http;
using Flash.Extensions.UidGenerator;
using Flash.Widgets.Configures;
using Flash.Widgets.DbContexts;
using Flash.Widgets.Models;
using Flash.Widgets.Models.TaobaoUtils;
using Masuit.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace Flash.Widgets.Controllers
{
    public class TaobaoUtilsController : BaseController
    {
        private readonly IUniqueIdGenerator _uniqueIdGenerator;
        private readonly IOfficeTools _officeTools;
        private readonly ICacheManager _cache;
        private readonly ILogger<TaobaoUtilsController> _logger;
        private readonly TaoBaoDbContext _dbContext;
        private readonly IHttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IOptionsMonitor<ExpressCodeConfigure> _optionsMonitor;
        private readonly IOptionsMonitor<SkuCodeConfigure> _optionsSkuCode;
        private readonly IOptionsMonitor<CostConfig> _optionCostConfig;
        private string PATH_TaobaoExport = "TaobaoExport";

        public TaobaoUtilsController(
            IUniqueIdGenerator uniqueIdGenerator,
            IOfficeTools officeTools,
            ICacheManager cache,
            ILogger<TaobaoUtilsController> logger,
            TaoBaoDbContext dbContext,
            IHttpClient httpClient,
            IConfiguration configuration,
            IOptionsMonitor<ExpressCodeConfigure> optionsMonitor,
            IOptionsMonitor<SkuCodeConfigure> optionsSkuCode,
            IOptionsMonitor<CostConfig> optionCostConfig)
        {
            this._uniqueIdGenerator = uniqueIdGenerator;
            this._officeTools = officeTools;
            this._cache = cache;
            this._logger = logger;
            this._dbContext = dbContext;
            this._httpClient = httpClient;
            this._configuration = configuration;
            this._optionsMonitor = optionsMonitor;
            this._optionsSkuCode = optionsSkuCode;
            this._optionCostConfig = optionCostConfig;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("Upload")]
        public async Task<long> Upload(IFormFile file)
        {
            Check.Argument.IsNotNull(file, nameof(file));

            var savePath = Path.Combine(Directory.GetCurrentDirectory(), PATH_TaobaoExport);
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            var cacheKeyUploadFile = "UploadFileName";
            var fileId = this._cache.HashGet<string>(cacheKeyUploadFile, file.FileName);
            if (!string.IsNullOrEmpty(fileId))
            {
                return fileId.Tolong();
            }
            else
            {
                fileId = this._uniqueIdGenerator.NewId().ToString();
            }
            this._cache.HashSet(cacheKeyUploadFile, file.FileName, fileId, TimeSpan.FromDays(7));
            
            var fileName = $"{fileId}.xlsx";
            using (var stream = new FileStream(Path.Combine(savePath, fileName),FileMode.Create))
            {
                await file.CopyToAsync(stream);
                this._logger.LogInformation($"文件上传成功：{fileName}");
                return fileId.Tolong();
            }
        }

        [HttpPost("InitBaseData")]
        public void InitBaseData()
        {
            this._cache.StringSet("SkuCodes", this._optionsSkuCode.CurrentValue.SkuCodes);
        }

        /// <summary>
        /// 导出Sku统一编码
        /// </summary>
        /// <returns></returns>
        [HttpPost("ExportSkuCode")]
        public FileResult ExportSkuCode()
        {
            var skuCodes = this._cache.StringGet<List<SkuInfo>>("SkuCodes");
            var sheetSetting = new SheetSetting
            {
                IsAutoNumber = true,
                HeaderRowHeight = 46,
                DataRowHeight = 32,
                DisplayGridlines = false
            };

            var buffer = this._officeTools.WriteExcelMultipleSheet(
                SheetInfo.Create("Sku编码", skuCodes, SkuInfo.GetHeaderColumns(), sheetSetting)
            );

            var fileName = $"Sku统一编码格式.xlsx";
            return File(buffer, "application/octet-stream", fileName);
        }

        /// <summary>
        /// 设置品牌单价
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("SetBrandUnitPrice")]
        public dynamic SetBrandUnitPrice(SetBrandUnitPriceRequestData requestData)
        {
            if (!requestData.Date.HasValue)
            {
                requestData.Date = DateTime.Now;
            }

            //单价设置
            var costConfig = new CostConfig
            {
                ExpressDelivery = requestData.ExpressDelivery,
                BrandUnitPrices = new Dictionary<string, decimal>()
            };

            requestData.BrandUnitPriceItems.ForEach(item =>
            {
                costConfig.BrandUnitPrices.Add(item.BrandCode, item.Price);
            });

            foreach (var item in this._optionCostConfig.CurrentValue.BrandUnitPrices)
            {
                costConfig.BrandUnitPrices.TryAdd(item.Key, item.Value);
            }

            var redisHashKey = $"{requestData.ShopId}:{requestData.CacheName}_CostConfigHash";
            this._cache.HashSet(redisHashKey, requestData.Date.Value.ToString("yyyy-MM-dd"), costConfig);
            this._logger.LogInformation($"设置品牌单价成功：{redisHashKey}");
            return new { Result = "YES" };
        }

        /// <summary>
        /// 获得品牌单价
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("GetBrandUnitPrice")]
        public GetBrandUnitPriceResponseData GetBrandUnitPrice(GetBrandUnitPriceRequestData requestData)
        {
            if (!requestData.Date.HasValue)
            {
                requestData.Date = DateTime.Now;
            }

            var redisHashKey = $"{requestData.ShopId}:{requestData.CacheName}_CostConfigHash";

            var result = this._cache.HashGet<CostConfig>(redisHashKey, $"{requestData.Date.Value.ToString("yyyy-MM-dd")}");
            //获得最后一次设置的单价
            var lastDates = this._cache.HashKeys<string>(redisHashKey).Select(p=> p?.ToDateTime()).Where(p=> p < requestData.Date).ToList();
            var lastDate = lastDates.OrderByDescending(p => p).FirstOrDefault();
            if (result == null && lastDate.HasValue)
            {
                result = this._cache.HashGet<CostConfig>(redisHashKey, $"{lastDate.Value.ToString("yyyy-MM-dd")}");
                this._logger.LogInformation($"获得最后一次设置的单价：{requestData.Date.Value.ToString("yyyy-MM-dd")} -> {redisHashKey}");
            }

            if (result == null)
            {
                ////单价设置
                //result = new CostConfig
                //{
                //    ExpressDelivery = 3,
                //    BrandUnitPrices = new Dictionary<string, decimal>()
                //};

                //result.BrandUnitPrices.Add("YITONG02", 41);
                //result.BrandUnitPrices.Add("ANYI01", 41);
                //result.BrandUnitPrices.Add("ANYI02", 41);
                //result.BrandUnitPrices.Add("YUJIAN02", 41);
                //result.BrandUnitPrices.Add("CAOBENXINGQIU02", 41);//草本星球二代

                result = this._optionCostConfig.CurrentValue;
            }
            else
            {
                foreach (var item in this._optionCostConfig.CurrentValue.BrandUnitPrices)
                {
                    result.BrandUnitPrices.TryAdd(item.Key, item.Value);
                }
            }

            this._cache.HashSet(redisHashKey, requestData.Date.Value.ToString("yyyy-MM-dd"), result);

            var items = new List<BrandUnitPriceItem>();
            foreach (var item in result.BrandUnitPrices)
            {
                items.Add(new BrandUnitPriceItem
                {
                    BrandCode = item.Key,
                    Price = item.Value
                });
            }

            

            return new GetBrandUnitPriceResponseData
            {
                ExpressDelivery = result.ExpressDelivery,
                CacheName = requestData.CacheName,
                BrandUnitPriceItems = items
            };
        }

        /// <summary>
        /// 计算每日对账
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("CalculateByDailyReconciliation")]
        public FileResult CalculateByDailyReconciliation([FromForm]CalculateByDailyReconciliationRequestData requestData)
        {
            Check.Argument.IsNotNull(requestData, nameof(requestData));
            Check.Argument.IsNotNull(requestData.OrderPrintDate, nameof(requestData.OrderPrintDate));

            this._logger.LogInformation($"计算每日对账：{requestData.ShopId} -> {requestData.OrderPrintDate.Value.ToString("yyyy-MM-dd")}");
            var orderPrintInfo = ReadExcelByOrderPrintLogger2PrintDate(requestData.OrderPrintDate, requestData.FileIdByOrderPrintLogger, obj => {
                return obj.PrintType == "平台订单" && obj.ExpressStatus == "使用中" && !obj.ShopDetails.Contains("【外发】") && !obj.ShopDetails.Contains("卖家备注：S");
            });

            var exportOrderInfos = ReadExcelByExportOrderInfo2OrderNumbers(requestData.FileIdByExportOrderInfo, orderPrintInfo.OrderNumbers);

            var shopCodes = this._cache.StringGet<List<SkuInfo>>("SkuCodes");
            var costConfig = CostConfig.Convert(GetBrandUnitPrice(new GetBrandUnitPriceRequestData
            {
                Date = requestData.OrderPrintDate,
                CacheName = "DUIZHANG",
                ShopId = requestData.ShopId
            }));

            var shopDetails = shopCodes.Select(p => new ShowNumberInfo { ShopName = p.SkuName, ShopCode = p.SkuCode, AliasCodes = p.AliasCodes, Number = 0 }).ToList();
            shopDetails.ForEach(item =>
            {
                //品牌单价
                if (costConfig.BrandUnitPrices.TryGetValue(item.ShopCode.Split('-')[0], out decimal costValue))
                {
                    item.UnitPrice = costValue;
                }

                var exportOrderInfo = exportOrderInfos.Where(p => p.ShopCode == item.ShopCode || item.AliasCodes.Contains(p.ShopCode));

                item.Number = exportOrderInfo.Sum(p => p.BuyNumber);
                item.CostAmount = item.Number * item.UnitPrice;
                item.SellAmount = exportOrderInfo.Where(p => p.ShopCode == item.ShopCode).Sum(p => p.BuyActualAmount);

                //利润额计算
                item.ProfitAmount = item.SellAmount - item.CostAmount;
            });

            //汇总计算
            var totalNumber = shopDetails.Sum(p => p.Number);
            var totalCostAmount = shopDetails.Sum(p => p.CostAmount);
            var totalSellAmount = shopDetails.Sum(p => p.SellAmount);
            var totalProfitAmount = shopDetails.Sum(p => p.ProfitAmount);
            var itemShowNumberInfo = new ShowNumberInfo
            {
                ShopName = "--------汇 总--------",
                Number = totalNumber,
                CostAmount = totalCostAmount,
                SellAmount = totalSellAmount,
                ProfitAmount = totalProfitAmount
            };
            shopDetails.Add(itemShowNumberInfo);

            var sheetSetting = new SheetSetting
            {
                IsAutoNumber = true,
                HeaderRowHeight = 46,
                DataRowHeight = 32,
                DisplayGridlines = false
            };

            var buffer = this._officeTools.WriteExcelMultipleSheet(
                SheetInfo.Create("汇总", new List<StatData> {
                    new StatData{
                        StatDate = requestData.OrderPrintDate.HasValue ? requestData.OrderPrintDate.Value.ToString("yyyy-MM-dd") : "",
                        SkuNumber = itemShowNumberInfo.Number,
                        CostAmount = itemShowNumberInfo.CostAmount,
                        SellAmount = itemShowNumberInfo.SellAmount,
                        ProfitAmount = itemShowNumberInfo.ProfitAmount - (orderPrintInfo.TotalPrintNumber * costConfig.ExpressDelivery),
                        FaceSheetNumber = orderPrintInfo.TotalPrintNumber,
                        FaceSheetAmount = orderPrintInfo.TotalPrintNumber * costConfig.ExpressDelivery,
                        ExpressDeliveryUnitPrice = costConfig.ExpressDelivery,
                        TotalExpenditureAmount = itemShowNumberInfo.CostAmount + (orderPrintInfo.TotalPrintNumber * costConfig.ExpressDelivery),
                        Remarks = string.Join("\n",costConfig.BrandUnitPrices.Select(p=> $"品牌代码：{p.Key} 单价：{p.Value}"))
                    }
                }, StatData.GetHeaderColumns(), sheetSetting),
                SheetInfo.Create("按SKU汇总", shopDetails, ShowNumberInfo.GetHeaderColumns(), sheetSetting),
                SheetInfo.Create("交易明细", exportOrderInfos, ExportOrderInfo.GetHeaderColumns(), sheetSetting),
                SheetInfo.Create("打印明细", orderPrintInfo.PrintLoggers.ToList(), OrderPrintLogger.GetHeaderColumns(), sheetSetting)
            );

            var fileId = this._uniqueIdGenerator.NewId();
            var fileName = $"全部对账_{fileId}.xlsx";
            if (requestData.OrderPrintDate.HasValue)
            {
                fileName = $"每日对账_{requestData.OrderPrintDate.Value.ToString("yyyy-MM-dd")}_{fileId}.xlsx";
            }
            return File(buffer, "application/octet-stream", fileName);
        }

        /// <summary>
        /// 计算实时对账
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("CalculateByRealTimeReconciliation")]
        public FileResult CalculateByRealTimeReconciliation([FromForm] CalculateByDailyReconciliationRequestData requestData)
        {
            Check.Argument.IsNotNull(requestData, nameof(requestData));

            var lastPrintDate = this._cache.StringGet<DateTime>($"{requestData.ShopId}:LastPrintDate");
            if (lastPrintDate == default(DateTime))
            {
                lastPrintDate = DateTime.Now.Date;
            }

            this._logger.LogInformation($"计算实时对账：{requestData.ShopId} -> {lastPrintDate.ToString("yyyy-MM-dd")}");
            var orderPrintInfo = ReadExcelByOrderPrintLogger2PrintDate(null, requestData.FileIdByOrderPrintLogger, obj => {
                return obj.PrintTime >= lastPrintDate &&
                    obj.PrintType == "平台订单" &&
                    obj.ExpressStatus == "使用中" &&
                    !obj.ShopDetails.Contains("【外发】") &&
                    !obj.ShopDetails.Contains("卖家备注：S");
            });

            var exportOrderInfos = ReadExcelByExportOrderInfo2OrderNumbers(requestData.FileIdByExportOrderInfo, orderPrintInfo.OrderNumbers);

            var shopCodes = this._cache.StringGet<List<SkuInfo>>("SkuCodes");
            var shopDetails = shopCodes.Select(p => new ShowNumberInfo { ShopName = p.SkuName, ShopCode = p.SkuCode, AliasCodes = p.AliasCodes, Number = 0 }).ToList();
            shopDetails.ForEach(item =>
            {
                var exportOrderInfo = exportOrderInfos.Where(p => p.ShopCode == item.ShopCode || item.AliasCodes.Contains(p.ShopCode));
                item.Number = exportOrderInfo.Sum(p => p.BuyNumber);
            });

            //汇总计算
            var totalNumber = shopDetails.Sum(p => p.Number);
            var itemShowNumberInfo = new ShowNumberInfo
            {
                ShopName = "--------汇 总--------",
                ShopCode ="",
                Number = totalNumber
            };
            shopDetails.Add(itemShowNumberInfo);

            var sheetSetting = new SheetSetting
            {
                IsAutoNumber = true,
                HeaderRowHeight = 46,
                DataRowHeight = 32,
                DisplayGridlines = false
            };

            var statHeaderColumns = new List<ExcelHeaderColumn>();
            statHeaderColumns.Add(ExcelHeaderColumn.Create("统计日期", "StatDate", "yyyy-MM-dd", 25));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("总支数", "SkuNumber", 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("总面单数", nameof(RealTimeReconciliation.FaceSheetNumber), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("面单数(圆通)", nameof(RealTimeReconciliation.FaceSheetYuanTong), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("面单数(中通)", nameof(RealTimeReconciliation.FaceSheetZhongTong), 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("面单数(顺丰)", nameof(RealTimeReconciliation.FaceSheetShunFen), 15));

            statHeaderColumns.Add(ExcelHeaderColumn.Create("安易总支数", "ANYI", 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("遇健总支数", "YUJIAN", 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("草本星球总支数", "CAOBENXINGQIU", 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("一同总支数", "YITONG", 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("吸呱总支数", "XIGUA", 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("颐健总支数", "YIJIAN", 15));
            statHeaderColumns.Add(ExcelHeaderColumn.Create("遇健盛世总支数", "YUJIANSHENGSHI", 15));
            //

            var shopDetailHeaderColumns = new List<ExcelHeaderColumn>();
            shopDetailHeaderColumns.Add(ExcelHeaderColumn.Create("SKU名称", nameof(ShowNumberInfo.ShopName), 20));
            shopDetailHeaderColumns.Add(ExcelHeaderColumn.Create("SKU编码", nameof(ShowNumberInfo.ShopCode), 20));
            shopDetailHeaderColumns.Add(ExcelHeaderColumn.Create("数量", nameof(ShowNumberInfo.Number), 10));

            var buffer = this._officeTools.WriteExcelMultipleSheet(
                SheetInfo.Create("汇总", new List<RealTimeReconciliation> {
                    new RealTimeReconciliation{
                        StatDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        SkuNumber = itemShowNumberInfo.Number,
                        FaceSheetNumber = orderPrintInfo.TotalPrintNumber,
                        FaceSheetYuanTong = orderPrintInfo.PrintLoggers.Where(p=> p.ExpressName == "圆通速递").Count(),
                        FaceSheetZhongTong = orderPrintInfo.PrintLoggers.Where(p=> p.ExpressName == "中通快递").Count(),
                        FaceSheetShunFen = orderPrintInfo.PrintLoggers.Where(p=> p.ExpressName == "顺丰速运").Count(),
                        ANYI = shopDetails.Where(p=> p.ShopCode.Contains("ANYI")).Sum(p=> p.Number),
                        YUJIAN = shopDetails.Where(p=> p.ShopCode.Contains("YUJIAN02")).Sum(p=> p.Number),
                        CAOBENXINGQIU = shopDetails.Where(p=> p.ShopCode.Contains("CAOBENXINGQIU")).Sum(p=> p.Number),
                        YITONG = shopDetails.Where(p=> p.ShopCode.Contains("YITONG")).Sum(p=> p.Number),
                        YIJIAN = shopDetails.Where(p=> p.ShopCode.Contains("YIJIAN")).Sum(p=> p.Number),
                        XIGUA = shopDetails.Where(p=> p.ShopCode.Contains("XIGUA")).Sum(p=> p.Number),
                        YUJIANSHENGSHI = shopDetails.Where(p=> p.ShopCode.Contains("YUJIANSHENGSHI")).Sum(p=> p.Number)
                    }
                }, statHeaderColumns, sheetSetting),
                SheetInfo.Create("按SKU汇总", shopDetails, shopDetailHeaderColumns, sheetSetting),
                SheetInfo.Create("打印明细", orderPrintInfo.PrintLoggers.ToList(), OrderPrintLogger.GetHeaderColumns(), sheetSetting)
            );

            var fileName = $"实时对账_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx";
            var file = File(buffer, "application/octet-stream", fileName);
            if (orderPrintInfo.PrintLoggers.Any())
            {
                lastPrintDate = orderPrintInfo.PrintLoggers.OrderByDescending(p => p.PrintTime).FirstOrDefault().PrintTime;
                this._cache.StringSet<DateTime?>($"{requestData.ShopId}:LastPrintDate", lastPrintDate.AddSeconds(1));
            }
            
            return file;
        }

        class RealTimeReconciliation
        {
            /// <summary>
            /// 统计日期
            /// </summary>
            public string StatDate { get; set; }
            /// <summary>
            /// SKU数量
            /// </summary>
            public int SkuNumber { get; set; }
            /// <summary>
            /// 面单数（快递单数）
            /// </summary>
            public int FaceSheetNumber { get; set; }
            public int FaceSheetYuanTong { get; set; }
            public int FaceSheetZhongTong { get; set; }
            public int FaceSheetShunFen { get; set; }
            public int ANYI { get; set; }
            public int YUJIAN { get; set; }
            public int CAOBENXINGQIU { get; set; }
            public int YITONG { get; set; }
            public int YIJIAN { get; set; }
            public int XIGUA { get; set; }
            public int YUJIANSHENGSHI { get; set; }
        }

        /// <summary>
        /// 顺丰快递对账单
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("ShunfenReconciliation")]
        public FileResult ShunfenReconciliation([FromForm] ShunfenReconciliationRequestData requestData)
        {
            Check.Argument.IsNotNull(requestData, nameof(requestData));

            var currDatetime = DateTime.Now;

            this._logger.LogInformation($"顺丰快递对账单：{requestData.ShopId} -> {currDatetime.ToString("yyyy-MM-dd")}");
            var orderPrintInfo = ReadExcelByOrderPrintLogger2PrintDate(null, requestData.FileIdByOrderPrintLogger, obj => {
                return 
                    obj.ExpressStatus == "使用中" &&
                    !obj.ShopDetails.Contains("【外发】") &&
                    !obj.ShopDetails.Contains("卖家备注：S");
            });

            var platformOrder = orderPrintInfo.PrintLoggers.Where(p => p.PrintType == "平台订单").ToList();
            var notPlatformOrder = orderPrintInfo.PrintLoggers.Where(p => p.PrintType == "自由打印").ToList();

            var shunfenList = ReadExcel<ShunfenReconciliationInfo>(requestData.FileIdByShunfen, ShunfenReconciliationInfo.GetHeaderColumns(),"Sheet1");
            var selfShunfenList = shunfenList.Where(p => orderPrintInfo.PrintLoggers.Select(p => p.ExpressNumber).Contains(p.ExpressNumber)).ToList();
            var selfShunfenPlatformOrderList = shunfenList.Where(p => platformOrder.Select(p => p.ExpressNumber).Contains(p.ExpressNumber)).ToList();
            var selfShunfenNotPlatformOrderList = shunfenList.Where(p => notPlatformOrder.Select(p => p.ExpressNumber).Contains(p.ExpressNumber)).ToList();

            var sheetSetting = new SheetSetting
            {
                IsAutoNumber = true,
                HeaderRowHeight = 46,
                DataRowHeight = 32,
                DisplayGridlines = false
            };

            var buffer = this._officeTools.WriteExcelMultipleSheet(
                SheetInfo.Create("全部-汇总", new List<ShunfenReconciliationState> {
                    new ShunfenReconciliationState{
                        TotalRowNumber = selfShunfenList.Count(),
                        TotalExpressNumber = selfShunfenList.Select(p=> p.ExpressNumber).Distinct().Count(),
                        TotalPayableAmount = selfShunfenList.Select(p=> p.PayableAmount).Sum()
                    }
                }, ShunfenReconciliationState.GetHeaderColumns(), sheetSetting),

                SheetInfo.Create("平台订单-汇总", new List<ShunfenReconciliationState> {
                    new ShunfenReconciliationState{
                        TotalRowNumber = selfShunfenPlatformOrderList.Count(),
                        TotalExpressNumber = selfShunfenPlatformOrderList.Select(p=> p.ExpressNumber).Distinct().Count(),
                        TotalPayableAmount = selfShunfenPlatformOrderList.Select(p=> p.PayableAmount).Sum()
                    }
                }, ShunfenReconciliationState.GetHeaderColumns(), sheetSetting),
                SheetInfo.Create("平台订单-顺丰快递费", selfShunfenPlatformOrderList, ShunfenReconciliationInfo.GetHeaderColumns(), sheetSetting),
                SheetInfo.Create("平台订单-打印明细", platformOrder, OrderPrintLogger.GetHeaderColumns(), sheetSetting),

                SheetInfo.Create("自由打印-汇总", new List<ShunfenReconciliationState> {
                    new ShunfenReconciliationState{
                        TotalRowNumber = selfShunfenNotPlatformOrderList.Count(),
                        TotalExpressNumber = selfShunfenNotPlatformOrderList.Select(p=> p.ExpressNumber).Distinct().Count(),
                        TotalPayableAmount = selfShunfenNotPlatformOrderList.Select(p=> p.PayableAmount).Sum()
                    }
                }, ShunfenReconciliationState.GetHeaderColumns(), sheetSetting),
                SheetInfo.Create("自由打印-顺丰快递费", selfShunfenNotPlatformOrderList, ShunfenReconciliationInfo.GetHeaderColumns(), sheetSetting),
                SheetInfo.Create("自由打印-打印明细", notPlatformOrder, OrderPrintLogger.GetHeaderColumns(), sheetSetting)
            );

            var fileName = $"顺丰快递对账单_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx";
            var file = File(buffer, "application/octet-stream", fileName);
            return file;
        }

        /// <summary>
        /// 计算每日盈利
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("CalculateByDailyProfit")]
        public FileResult CalculateByDailyProfit([FromForm] CalculateByDailyProfitRequestData requestData)
        {
            //交易记录
            var exportOrderInfos = ReadExcelByExportOrderInfo(requestData.FileIdByExportOrderInfo).Where(p=>
                p.OrderStatus != "交易关闭" &&
                p.OrderStatus != "等待买家付款");

            if (!requestData.StatDate.HasValue)
            {
                requestData.StatDate = DateTime.Now.Date;
            }

            //指定付款日期
            exportOrderInfos = exportOrderInfos.Where(p => p.OrderPaymentDate == requestData.StatDate);

            exportOrderInfos = exportOrderInfos.Where(p =>
                string.IsNullOrEmpty(p.MerchantRemarks) ||
                (!p.MerchantRemarks.Contains("代") &&
                !p.MerchantRemarks.ToUpper().Contains("S"))).ToList();

            var shopCodes = this._cache.StringGet<List<SkuInfo>>("SkuCodes");
            var costConfig = CostConfig.Convert(GetBrandUnitPrice(new GetBrandUnitPriceRequestData
            {
                Date = requestData.StatDate,
                CacheName = "DUIZHANG",
                ShopId = requestData.ShopId
            }));

            var shopDetails = shopCodes.Select(p => new ShowNumberInfo { ShopName = p.SkuName, ShopCode = p.SkuCode, AliasCodes = p.AliasCodes, Number = 0 }).ToList();
            //所有交易记录按SKU级汇总
            shopDetails.ForEach(item =>
            {
                //品牌单价
                if (costConfig.BrandUnitPrices.TryGetValue(item.ShopCode.Split('-')[0], out decimal costValue))
                {
                    item.UnitPrice = costValue;
                }

                var tmp = exportOrderInfos.Where(p => p.ShopCode == item.ShopCode || item.AliasCodes.Contains(p.ShopCode)).ToList();

                item.Number = tmp.Sum(p => p.BuyNumber);
                item.CostAmount = item.Number * item.UnitPrice;
                item.SellAmount = tmp.Sum(p => p.BuyActualAmount);

                //利润额计算
                item.ProfitAmount = item.SellAmount - item.CostAmount;
            });


            //汇总计算
            var totalNumber = shopDetails.Sum(p => p.Number);
            var totalCostAmount = shopDetails.Sum(p => p.CostAmount);
            var totalSellAmount = shopDetails.Sum(p=> p.SellAmount);
            var totalProfitAmount = shopDetails.Sum(p => p.ProfitAmount);
            var itemShowNumberInfo = new ShowNumberInfo
            {
                ShopName = "--------汇 总--------",
                Number = totalNumber,
                CostAmount = totalCostAmount,
                SellAmount = totalSellAmount,
                ProfitAmount = totalProfitAmount
            };
            shopDetails.Add(itemShowNumberInfo);

            var sheetSetting = new SheetSetting
            {
                IsAutoNumber = true,
                HeaderRowHeight = 46,
                DataRowHeight = 32,
                DisplayGridlines = false
            };

            var buffer = this._officeTools.WriteExcelMultipleSheet(
                SheetInfo.Create("今日SKU汇总", shopDetails, ShowNumberInfo.GetHeaderColumns(), sheetSetting),
                SheetInfo.Create("今日交易明细", exportOrderInfos.ToList(), ExportOrderInfo.GetHeaderColumns(), sheetSetting)
            );

            var fileId = this._uniqueIdGenerator.NewId();
            var fileName = "";
            if (requestData.StatDate.HasValue)
            {
                fileName = $"每日盈利_{requestData.StatDate.Value.ToString("yyyy-MM-dd")}_{fileId}.xlsx";
            }
            else
            {
                var endData = exportOrderInfos.Select(p => p.OrderPaymentDate).Max();
                var startData = exportOrderInfos.Select(p => p.OrderPaymentDate).Max();

                fileName = $"{startData.ToString("yyyy-MM-dd")}至{endData.ToString("yyyy-MM-dd")}_{fileId}.xlsx";
            }
            return File(buffer, "application/octet-stream", fileName);
        }

        /// <summary>
        /// 计算盈利统计
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("CalculateByCount")]
        public ActionResult CalculateByCount([FromForm] CalculateByCountRequestData requestData)
        {
            //if (string.IsNullOrEmpty(requestData.FileIds)) return base.NotFound();
            var fileIds = requestData.FileIds;
            if (!fileIds.Any()) return base.NotFound();

            fileIds = fileIds.Select(p => p.Trim()).ToList();
            var statDataHeaderColumns = StatData.GetHeaderColumns();
            var listStatData = new List<StatData>();
            fileIds.ForEach(item =>
            {
                listStatData.AddRange(ReadExcel<StatData>(item.Tolong(), statDataHeaderColumns,"汇总"));
            });

            var sheetSetting = new SheetSetting
            {
                IsAutoNumber = true,
                HeaderRowHeight = 46,
                DataRowHeight = 32,
                DisplayGridlines = false
            };

            var skuNumber = listStatData.Sum(p=> p.SkuNumber);
            var costAmount = listStatData.Sum(p => p.CostAmount);
            var sellAmount = listStatData.Sum(p => p.SellAmount);
            var profitAmount = listStatData.Sum(p => p.ProfitAmount);
            var faceSheetNumber = listStatData.Sum(p => p.FaceSheetNumber);
            var faceSheetAmount = listStatData.Sum(p => p.FaceSheetAmount);
            var promotionFee = listStatData.Sum(p => p.PromotionFee);
            var DF_Fee = listStatData.Sum(p => p.DF_Fee);

            var buffer = this._officeTools.WriteExcelMultipleSheet(
                SheetInfo.Create("汇总", new List<CalculateByCountInfo> {
                    new CalculateByCountInfo{
                        SkuNumber = skuNumber,
                        CostAmount = costAmount,
                        SellAmount = sellAmount,
                        ProfitAmount = profitAmount,
                        FaceSheetNumber = faceSheetNumber,
                        FaceSheetAmount = faceSheetAmount,
                        PromotionFee = promotionFee,
                        DF_Fee = DF_Fee
                    }
                }, CalculateByCountInfo.GetHeaderColumns(), sheetSetting),
                SheetInfo.Create("每日汇总明细", listStatData, StatData.GetHeaderColumns(), sheetSetting)
            );

            var fileId = this._uniqueIdGenerator.NewId();
            return File(buffer, "application/octet-stream", $"盈利全部汇总_{fileId}.xlsx");
        }

        /// <summary>
        /// 计算每日代发
        /// </summary>
        /// <returns></returns>
        [HttpPost("CalculateByDF")]
        public FileResult CalculateByDF([FromForm] CalculateByDFRequestData requestData)
        {
            //交易记录
            var exportOrderInfos = ReadExcelByExportOrderInfo(requestData.FileIdByExportOrderInfo);
            if (requestData.StatDate.HasValue)
            {
                //指定付款日期
                exportOrderInfos = exportOrderInfos.Where(p => p.DeliveryDate == requestData.StatDate).ToList();
            }

            //代发交易记录
            var df_OrderInfos = new List<ExportOrderInfo>();
            exportOrderInfos.ForEach(item =>
            {
                if (!string.IsNullOrWhiteSpace(item.MerchantRemarks) && item.MerchantRemarks.Trim() == "代")
                {
                    df_OrderInfos.Add(item);
                }
            });

            var shopCodes = this._cache.StringGet<List<SkuInfo>>("SkuCodes");
            var costConfig = CostConfig.Convert(GetBrandUnitPrice(new GetBrandUnitPriceRequestData
            {
                Date = requestData.StatDate,
                CacheName = "DAIFA",
                ShopId = requestData.ShopId
            }));

            var shopDetails = shopCodes.Select(p => new ShowNumberInfo { ShopName = p.SkuName, ShopCode = p.SkuCode, AliasCodes = p.AliasCodes, Number = 0 }).ToList();
            shopDetails.ForEach(item =>
            {
                //品牌单价
                if (costConfig.BrandUnitPrices.TryGetValue(item.ShopCode.Split('-')[0], out decimal costValue))
                {
                    item.UnitPrice = costValue;
                }

                var df_OrderInfo = df_OrderInfos.Where(p => p.ShopCode == item.ShopCode || item.AliasCodes.Contains(p.ShopCode));

                item.Number = df_OrderInfo.Sum(p => p.BuyNumber);
                item.CostAmount = item.Number * item.UnitPrice;
                item.SellAmount = df_OrderInfo.Sum(p => p.BuyActualAmount);

                //利润额计算
                item.ProfitAmount = item.SellAmount - item.CostAmount;
            });

            //汇总计算
            var count_MainOrderNumber = df_OrderInfos.Select(p => p.MainOrderNumber).Distinct().Count();
            var totalNumber = shopDetails.Sum(p => p.Number);
            var totalCostAmount = shopDetails.Sum(p => p.CostAmount);
            var totalSellAmount = shopDetails.Sum(p => p.SellAmount);
            var totalProfitAmount = shopDetails.Sum(p => p.ProfitAmount);
            shopDetails.Add(new ShowNumberInfo
            {
                ShopName = "--------汇 总--------",
                ShopCode = $"快递单数：{count_MainOrderNumber}单  快递单价：{costConfig.ExpressDelivery}",
                Number = totalNumber,
                CostAmount = totalCostAmount,
                SellAmount = totalSellAmount,
                ProfitAmount = totalProfitAmount
            });

            shopDetails.Add(new ShowNumberInfo
            {
                ShopName = "--------汇 总--------",
                ShopCode = $"快递费：{costConfig.ExpressDelivery* count_MainOrderNumber}",
            });

            shopDetails.Add(new ShowNumberInfo
            {
                ShopName = "--------汇 总--------",
                ShopCode = $"待付款总额：{(costConfig.ExpressDelivery * count_MainOrderNumber)+ totalCostAmount}",
            });

            var sheetSetting = new SheetSetting
            {
                IsAutoNumber = true,
                HeaderRowHeight = 46,
                DataRowHeight = 32,
                DisplayGridlines = false
            };

            var buffer = this._officeTools.WriteExcelMultipleSheet(
                SheetInfo.Create("SKU汇总", shopDetails, ShowNumberInfo.GetHeaderColumns(), sheetSetting),
                SheetInfo.Create("交易明细", df_OrderInfos, ExportOrderInfo.GetHeaderColumns(), sheetSetting)
            );

            var fileId = this._uniqueIdGenerator.NewId();
            var fileName = $"全部代发_{fileId}.xlsx";
            if (requestData.StatDate.HasValue)
            {
                fileName = $"每日代发_{requestData.StatDate.Value.ToString("yyyy-MM-dd")}_{fileId}.xlsx";
            }
            return File(buffer, "application/octet-stream", fileName);
        }

        /// <summary>
        /// 导出代发订单
        /// </summary>
        /// <returns></returns>
        [HttpPost("ExprotDaiFaOrderList")]
        public FileResult ExprotDaiFaOrderList([FromForm] ExprotDaiFaOrderListRequestData requestData)
        {
            var sheetSetting = new SheetSetting
            {
                IsAutoNumber = false,
                HeaderRowHeight = 46,
                DataRowHeight = 32,
                DisplayGridlines = false
            };

            var buffer = this._officeTools.WriteExcelMultipleSheet(
                SheetInfo.Create("Sheet1", requestData.Items, ExprotDaiFaOrderItem.GetHeaderColumns(), sheetSetting)
            );

            var fileId = this._uniqueIdGenerator.NewId();
            var fileName = $"代发订单_{DateTime.Now.ToString("yyyy-MM-dd")}_{fileId}.xlsx";
            return File(buffer, "application/octet-stream", fileName);
        }

        /// <summary>
        /// 计算代发运单
        /// </summary>
        /// <returns></returns>
        [HttpPost("CalculateByDaiFanYunDan")]
        public FileResult CalculateByDaiFanYunDan([FromForm] CalculateByDaiFanYunDanRequestData requestData)
        {
            //订单打印日志
            var orderPrintInfo = ReadExcelByOrderPrintLogger2PrintDate(requestData.Date, requestData.FileIdByOrderPrint);

            var daiFaOrderItems = ReadExcel<ExprotDaiFaOrderItem>(requestData.FileIdByDaiFa, ExprotDaiFaOrderItem.GetHeaderColumns());

            var list = new List<DaiFanYunDanInfo>();
            daiFaOrderItems.ForEach(item =>
            {
                var orderPrint = orderPrintInfo.PrintLoggers.FirstOrDefault(p => p.ShopDetails.Contains(item.Commodity.Trim()));
                if (orderPrint != null)
                {
                    list.Add(new DaiFanYunDanInfo
                    {
                        MainOrderNumber = item.Commodity,
                        ExpressNumber = orderPrint.ExpressNumber,
                        ExpressCode = requestData.ExpressCode ?? ""
                    });
                }
            });

            var sheetSetting = new SheetSetting
            {
                IsAutoNumber = false,
                HeaderRowHeight = 46,
                DataRowHeight = 32,
                DisplayGridlines = false
            };

            var buffer = this._officeTools.WriteExcelMultipleSheet(
                SheetInfo.Create("sheet1", list, DaiFanYunDanInfo.GetHeaderColumns(), sheetSetting)
            );

            var fileId = this._uniqueIdGenerator.NewId();
            var fileName = $"批量发货_{DateTime.Now.ToString("yyyy-MM-dd")}_{fileId}.xlsx";
            return File(buffer, "application/octet-stream", fileName);
        }


        /// <summary>
        /// 统计SKU销量
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("ExportShopStatic")]
        public List<ShowNumberInfo> ExportShopStatic([FromBody] ExportShopStaticRequestData requestData)
        {
            if (!requestData.StartTime.HasValue) requestData.StartTime = DateTime.Now.Date;
            if (!requestData.EndTime.HasValue) requestData.EndTime = DateTime.Now.Date;

            //交易记录
            var exportOrderInfos = ReadExcelByExportOrderInfo(requestData.FileIdByExportOrderInfo).AsEnumerable();
            exportOrderInfos = exportOrderInfos.Where(p => p.OrderCreateTime >= requestData.StartTime && p.OrderCreateTime <= requestData.EndTime.Value.AddDays(1).AddSeconds(-1));

            var shopCodes = this._cache.StringGet<List<SkuInfo>>("SkuCodes");
            var shopDetails = shopCodes.Select(p => new ShowNumberInfo { ShopName = p.SkuName, ShopCode = p.SkuCode, AliasCodes = p.AliasCodes, Number = 0 }).ToList();
            shopDetails.ForEach(item =>
            {
                var df_OrderInfo = exportOrderInfos.Where(p => p.ShopCode == item.ShopCode || item.AliasCodes.Contains(p.ShopCode));

                item.Number = df_OrderInfo.Sum(p => p.BuyNumber);
            });

            return shopDetails;
        }

        /// <summary>
        /// 订单时段分析
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("CalculateByOrder2TimeInterval")]
        public List<CalculateByOrder2TimeInterval> CalculateByOrder2TimeInterval([FromBody] CalculateByOrder2TimeIntervalRequestData requestData)
        {
            var list = new List<CalculateByOrder2TimeInterval>();
            for (int i = 0; i < 24; i++)
            {
                list.Add(new CalculateByOrder2TimeInterval
                {
                    TimeInterval = i,
                    SellAmount = 0,
                    SellNumber = 0  
                });
            }

            if (!requestData.StartDate.HasValue) requestData.StartDate = DateTime.Now.Date;
            if (!requestData.EndDate.HasValue) requestData.EndDate = DateTime.Now.Date;

            //交易记录
            var exportOrderInfos = ReadExcelByExportOrderInfo(requestData.FileIdByExportOrderInfo).AsEnumerable();
            exportOrderInfos = exportOrderInfos.Where(p => p.OrderCreateTime >= requestData.StartDate && p.OrderCreateTime <= requestData.EndDate.Value.AddDays(1).AddSeconds(-1));

            list.ForEach(item =>
            {
                var tmp = exportOrderInfos.Where(p => p.OrderCreateTime.Hour == item.TimeInterval).ToList();
                item.SellNumber = tmp.Count();
                item.SellAmount = tmp.Sum(p => p.BuyActualAmount);
            });

            return list;
        }

        /// <summary>
        /// 销售订单仅退款（极速退款）
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("CalculateByOrder2Refund")]
        public List<CalculateByOrder2RefundResponseData> CalculateByOrder2Refund([FromBody] CalculateByOrder2RefundRequestData requestData)
        {
            if (!requestData.StartDate.HasValue) requestData.StartDate = DateTime.Now.Date;
            if (!requestData.EndDate.HasValue) requestData.EndDate = DateTime.Now.Date;

            //订单打印日志
            var orderPrintInfo = ReadExcelByOrderPrintLogger2PrintDate(null, requestData.FileIdByOrderPrintLogger, new List<string> { "平台订单" });
            //交易记录
            var exportOrderInfos = ReadExcelByExportOrderInfo2OrderNumbers(requestData.FileIdByExportOrderInfo,orderPrintInfo.OrderNumbers).AsEnumerable();
            exportOrderInfos = exportOrderInfos.Where(p => p.RefundStatus == "退款成功" && p.OrderCreateTime >= requestData.StartDate && p.OrderCreateTime <= requestData.EndDate.Value.AddDays(1).AddSeconds(-1));

            //面单打印记录
            var printLoggers = orderPrintInfo.PrintLoggers.Where(p => exportOrderInfos.Select(s => s.MainOrderNumber).Contains(p.OrderNumber));
                        
            var list = new List<CalculateByOrder2RefundResponseData>();
            foreach (var item in exportOrderInfos)
            {
                var expressNumber = printLoggers.FirstOrDefault(p => p.OrderNumber.Contains(item.MainOrderNumber))?.ExpressNumber ?? "";
                if (!string.IsNullOrEmpty(expressNumber))
                {
                    list.Add(new CalculateByOrder2RefundResponseData
                    {
                        OrderNumber = item.MainOrderNumber,
                        ExpressNumber = expressNumber,
                        RefundStatus = item.RefundStatus,
                        BuyActualAmount = item.BuyActualAmount
                    });
                }
            }
            
            return list;
        }

        /// <summary>
        /// 销售订单物流跟踪导入
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("SellOrder2LogisticsTrackingImport")]
        public List<SellOrder2LogisticsTrackingResponseData> SellOrder2LogisticsTrackingImport([FromBody] SellOrder2LogisticsTrackingImportRequestData requestData)
        {
            //交易记录
            var exportOrderInfos = ReadExcelByExportOrderInfo(requestData.FileIdByExportOrderInfo).Where(p => p.RefundStatus == "退款成功" && (!string.IsNullOrWhiteSpace(p.ExpressNumberStr) && p.ExpressNumberStr != "null")).ToList();
                        
            var list = exportOrderInfos.Select(item=> new SellOrder2LogisticsTrackingResponseData
            {
                MainOrderNumber = item.MainOrderNumber,
                ExpressNumber = item.ExpressNumberStr.Replace("No:", ""),
                ExpressName = item.ExpressName,
                MerchantRemarks = item.MerchantRemarks ?? ""
            }).ToList();
            var insertList = new List<LogisticsTrackingEntity>();
            var entityList = this._dbContext.Set<LogisticsTrackingEntity>().Where(p => p.ShopId == requestData.ShopId && list.Select(p => p.ExpressNumber).Contains(p.ExpressNumber)).ToList();

            insertList = list.Where(p => !entityList.Select(p => p.ExpressNumber).Contains(p.ExpressNumber)).Select(p => new LogisticsTrackingEntity
            {
                Id = this._uniqueIdGenerator.NewId(),
                CreateTime = DateTime.Now,
                LastModifyTime = DateTime.Now,
                CreateUserId = 0,
                LastModifyUserId = 0,
                ExpressName = p.ExpressName,
                ExpressNumber = p.ExpressNumber,
                MainOrderNumber = p.MainOrderNumber,
                MerchantRemarks = p.MerchantRemarks,
                State = LogisticsTrackingState.WaitHandle,
                IsDelete = false,
                ShopId = p.ShopId
            }).ToList();
            if (insertList != null && insertList.Any())
            {
                this._dbContext.AddRange(insertList);
                this._dbContext.SaveChanges();
            }

            return list;
        }

        /// <summary>
        /// 获得销售订单物流跟踪分页数据
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("SellOrder2LogisticsTrackingGetList")]
        public async Task<PageCountResponse<SellOrder2LogisticsTrackingGetListResponseData>> SellOrder2LogisticsTrackingGetList([FromBody] SellOrder2LogisticsTrackingGetListRequestData requestData)
        {
            var queryable = this._dbContext.Set<LogisticsTrackingEntity>().Where(p=> p.ShopId == requestData.ShopId && !p.IsDelete)
                .WhereWith(requestData, l => l.MainOrderNumber, r => r.MainOrderNumber, OperatorType.Like)
                .WhereWith(requestData, l => l.ExpressNumber, r => r.ExpressNumber, OperatorType.Like)
                .WhereWith(requestData, l => l.ExpressName, r => r.ExpressName, OperatorType.Equal)
                .WhereWith(requestData, l => l.State, r => r.State, OperatorType.Equal);

            var result = await queryable.QueryPageAsync<LogisticsTrackingEntity>(requestData, (t, c) => c.Add(OrderBy.Create(t, s => s.CreateTime, PageOrderBy.DESC)), isCount: true) as PageCountResponse<LogisticsTrackingEntity>;
            if (result != null && result.List != null && result.List.Any())
            {
                return new PageCountResponse<SellOrder2LogisticsTrackingGetListResponseData>(result.List.Select(p => new SellOrder2LogisticsTrackingGetListResponseData
                {
                    Id = p.Id,
                    CreateTime = p.CreateTime,
                    CreateUserId = p.CreateUserId,
                    ExpressName = p.ExpressName,
                    ExpressNumber = p.ExpressNumber,
                    Phone = p.Phone,
                    IsDelete = p.IsDelete,
                    LastModifyTime = p.LastModifyTime,
                    LastModifyUserId = p.LastModifyUserId,
                    MainOrderNumber = p.MainOrderNumber,
                    MerchantRemarks = p.MerchantRemarks,
                    State = p.State,
                    LogisticsTracking = p.LogisticsTracking,
                    BusinessTime = p.BusinessTime,
                    RetryCount = p.RetryCount,
                    SystemRemarks = p.SystemRemarks,
                    ShopId = p.ShopId
                }).ToList(), result.PageIndex, result.PageSize, result.Total);
            }

            return new PageCountResponse<SellOrder2LogisticsTrackingGetListResponseData>(null, requestData.PageIndex, requestData.PageSize, 0);
        }

        /// <summary>
        /// 销售订单物流跟踪-标记已完成
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("SellOrder2LogisticsTrackingSetCompleted")]
        public async Task<ResultResponse> SellOrder2LogisticsTrackingSetCompleted([FromBody] SellOrder2LogisticsTrackingSetCompletedRequestData requestData)
        {
            var info = await this._dbContext.Set<LogisticsTrackingEntity>().FirstOrDefaultAsync(p => p.Id == requestData.Id);
            if (info == null)
            {
                return ResultResponse.Create(-1, $"记录不存在");
            }

            info.State = LogisticsTrackingState.CompletedTrack;
            if( this._dbContext.SaveChanges() > 0)
            {
                return ResultResponse.Create(0, $"成功");
            }

            return ResultResponse.Create(-1, $"失败");
        }

        /// <summary>
        /// 销售订单物流跟踪-删除
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("SellOrder2LogisticsTrackingSetDelete")]
        public async Task<ResultResponse> SellOrder2LogisticsTrackingSetDelete([FromBody] SellOrder2LogisticsTrackingSetCompletedRequestData requestData)
        {
            var info = await this._dbContext.Set<LogisticsTrackingEntity>().FirstOrDefaultAsync(p => p.Id == requestData.Id);
            if (info == null)
            {
                return ResultResponse.Create(-1, $"记录不存在");
            }

            info.IsDelete = true;
            info.LastModifyTime = DateTime.Now;
            if (this._dbContext.SaveChanges() > 0)
            {
                return ResultResponse.Create(0, $"成功");
            }

            return ResultResponse.Create(-1, $"失败");
        }

        /// <summary>
        /// 销售订单物流跟踪-查询
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("SellOrder2LogisticsTrackingSearch")]
        public async Task<string> SellOrder2LogisticsTrackingSearch([FromBody] SellOrder2LogisticsTrackingSearchRequestData requestData)
        {
            var info = await this._dbContext.Set<LogisticsTrackingEntity>().FirstOrDefaultAsync(p => p.Id == requestData.Id);
            if (info == null)
            {
                return "";
            }

            var expressCodeInfo = this._optionsMonitor.CurrentValue.ExpressCodes.FirstOrDefault(p => p.ExpressName == info.ExpressName);
            if (expressCodeInfo == null)
            {
                return "";
            }
            var request = new
            {
                com = expressCodeInfo.KuaiDi100Code,
                num = info.ExpressNumber.Replace("No:", ""),
                phone = info.Phone
            };
            var param = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var customer = this._configuration["KuaiDi100:Customer"];
            var sign = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes($"{param}{this._configuration["KuaiDi100:Key"]}{customer}")));//param+key+customer
            
            var result = await this._httpClient.PostAsync($"https://poll.kuaidi100.com/poll/query.do?customer={customer}&sign={sign}&param={param}", new { });
            var trackingResult = await result.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(trackingResult))
            {
                info.LogisticsTracking = trackingResult;
                this._dbContext.SaveChanges();
            }
            return trackingResult;
        }

        /// <summary>
        /// 销售订单物流跟踪-新增
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("SellOrder2LogisticsTrackingAddInfo")]
        public async Task<ResultResponse> SellOrder2LogisticsTrackingAddInfo([FromBody] SellOrder2LogisticsTrackingAddInfoRequestData requestData)
        {
            if (string.IsNullOrWhiteSpace(requestData.MainOrderNumber)|| string.IsNullOrWhiteSpace(requestData.ExpressNumber)|| string.IsNullOrWhiteSpace(requestData.ExpressName))
            {
                return ResultResponse.Create(-1, $"必填不允许为空");
            }


            var logisticsTrackingEntity = default(LogisticsTrackingEntity);
            //编辑
            if (requestData.Id.HasValue)
            {
                logisticsTrackingEntity = await this._dbContext.Set<LogisticsTrackingEntity>().FirstOrDefaultAsync(p => p.Id == requestData.Id && p.ShopId == requestData.ShopId);
                if (logisticsTrackingEntity ==null)
                {
                    return ResultResponse.Create(-1, $"数据不存在");
                }

                var hasInfo = await this._dbContext.Set<LogisticsTrackingEntity>().FirstOrDefaultAsync(p => p.ShopId == requestData.ShopId && p.ExpressNumber == requestData.ExpressNumber && p.Id != logisticsTrackingEntity.Id);
                if (hasInfo != null)
                {
                    return ResultResponse.Create(-1, $"物流单号[{requestData.ExpressNumber}]已存在");
                }

                logisticsTrackingEntity.LastModifyTime = DateTime.Now;
                logisticsTrackingEntity.ExpressNumber = requestData.ExpressNumber;
                logisticsTrackingEntity.MainOrderNumber = requestData.MainOrderNumber;
                logisticsTrackingEntity.MerchantRemarks = requestData.MerchantRemarks ?? "";
                logisticsTrackingEntity.Phone = requestData.Phone ?? "";
                this._dbContext.Update(logisticsTrackingEntity);

                if (this._dbContext.SaveChanges() > 0)
                {
                    return ResultResponse.Create(0, $"成功");
                }
            }
            else
            {
                var info = await this._dbContext.Set<LogisticsTrackingEntity>().FirstOrDefaultAsync(p => p.ExpressNumber == requestData.ExpressNumber && p.ShopId == requestData.ShopId);
                if (info != null)
                {
                    return ResultResponse.Create(-1, $"物流单号[{requestData.ExpressNumber}]已存在");
                }

                this._dbContext.Add(new LogisticsTrackingEntity
                {
                    Id = this._uniqueIdGenerator.NewId(),
                    CreateTime = DateTime.Now,
                    LastModifyTime = DateTime.Now,
                    CreateUserId = 0,
                    LastModifyUserId = 0,
                    ExpressName = requestData.ExpressName,
                    ExpressNumber = requestData.ExpressNumber,
                    Phone = requestData.Phone ?? "",
                    MainOrderNumber = requestData.MainOrderNumber,
                    MerchantRemarks = requestData.MerchantRemarks ?? "",
                    State = LogisticsTrackingState.WaitHandle,
                    IsDelete = false,
                    ShopId = requestData.ShopId
                });

                if (this._dbContext.SaveChanges() > 0)
                {
                    return ResultResponse.Create(0, $"成功");
                }
            }
            
            return ResultResponse.Create(-1, $"失败");
        }

        /// <summary>
        /// 快递公司列表
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("ExpressList")]
        public  ResultResponse<Object> ExpressList()
        {
            return ResultResponse<Object>.Create<Object>(-1, this._optionsMonitor.CurrentValue.ExpressCodes.Select(p => p.ExpressName).ToList());
        }

        /// <summary>
        /// 推广费用导入
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("PromotionFreeItemImport")]
        public async Task<ResultResponse<List<PromotionFreeItemEntity>>> PromotionFreeItemImport([FromBody] PromotionFreeItemImportRequestData requestData)
        {
            var promotionFreeItems = ReadExcel<PromotionFreeItemEntity>(requestData.FileId, PromotionFreeItemEntity.GetHeaderColumns());
            var transactionDates = promotionFreeItems.Select(p => p.TransactionDate).Distinct();
            this._dbContext.Set<PromotionFreeItemEntity>().RemoveRange(this._dbContext.Set<PromotionFreeItemEntity>().Where(p => transactionDates.Contains(p.TransactionDate)));
            this._dbContext.AddRange(promotionFreeItems);

            await this._dbContext.SaveChangesAsync();
            return ResultResponse<List<PromotionFreeItemEntity>>.Create(0, promotionFreeItems);
        }

        /// <summary>
        /// 读取Excel数据（交易订单数据）
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        private List<ExportOrderInfo> ReadExcelByExportOrderInfo(long fileId)
        {
            var excelPath = Path.Combine(Directory.GetCurrentDirectory(), PATH_TaobaoExport, $"{fileId}.xlsx");
            var file = System.IO.File.ReadAllBytes(excelPath);
            return this._officeTools.ReadExcel<ExportOrderInfo>(file, ExportOrderInfo.GetHeaderColumns());
        }

        /// <summary>
        /// 读取Excel数据（交易订单数据）
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="orderNumbers"></param>
        /// <returns></returns>
        private List<ExportOrderInfo> ReadExcelByExportOrderInfo2OrderNumbers(long fileId, List<string> orderNumbers)
        {
            var excelList = ReadExcelByExportOrderInfo(fileId);
            return excelList.Where(p => p.OrderStatus != "交易关闭" && orderNumbers.Contains(p.MainOrderNumber)).ToList();
        }

        /// <summary>
        /// 读取Excel数据（打印日志）
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        private List<OrderPrintLogger> ReadExcelByOrderPrintLogger(long fileId)
        {
            var excelPath = Path.Combine(Directory.GetCurrentDirectory(), PATH_TaobaoExport, $"{fileId}.xlsx");
            var file = System.IO.File.ReadAllBytes(excelPath);
            return this._officeTools.ReadExcel<OrderPrintLogger>(file, OrderPrintLogger.GetHeaderColumns());
        }

        /// <summary>
        /// 读取Excel数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileId"></param>
        /// <param name="columns"></param>
        /// <param name="sheelName"></param>
        /// <returns></returns>
        private List<T> ReadExcel<T>(long fileId, List<ExcelHeaderColumn> columns, string sheelName = "") where T : new()
        {
            var excelPath = Path.Combine(Directory.GetCurrentDirectory(), PATH_TaobaoExport, $"{fileId}.xlsx");
            var file = System.IO.File.ReadAllBytes(excelPath);
            return this._officeTools.ReadExcel<T>(file, sheelName, columns);
        }

        /// <summary>
        /// 读取Excel数据（打印日志）
        /// </summary>
        /// <param name="printDate"></param>
        /// <param name="fileId"></param>
        /// <param name="printTypes"></param>
        /// <returns></returns>
        private (int TotalPrintNumber, List<string> OrderNumbers, IEnumerable<OrderPrintLogger> PrintLoggers) ReadExcelByOrderPrintLogger2PrintDate(DateTime? printDate, long fileId, List<string> printTypes = null, List<string> expressStatus = null)
        {
            var excelList = ReadExcelByOrderPrintLogger(fileId).AsEnumerable();
            
            if (printDate.HasValue)
            {
                excelList = excelList.Where(p => p.PrintDate == printDate.Value);
            }

            if (printTypes != null && printTypes.Any())
            {
                excelList = excelList.Where(p => printTypes.Contains(p.PrintType));
            }

            if (expressStatus != null && expressStatus.Any())
            {
                excelList = excelList.Where(p => expressStatus.Contains(p.ExpressStatus));
            }

            List<OrderPrintLogger> resultExeclList = new List<OrderPrintLogger>();
            foreach (var item in excelList)
            {
                if (!resultExeclList.Any(p => p.ExpressNumber == item.ExpressNumber))
                {
                    resultExeclList.Add(item);
                }
            }

            return (resultExeclList.Count(),
                resultExeclList.SelectMany(p => string.IsNullOrWhiteSpace(p.OrderNumber) ? new string[0] : (p.OrderNumber.Split(",\n")))
                .Distinct().ToList(), resultExeclList);
        }

        /// <summary>
        /// 读取Excel数据（打印日志）
        /// </summary>
        /// <param name="printDate"></param>
        /// <param name="fileId"></param>
        /// <param name="printTypes"></param>
        /// <returns></returns>
        private (int TotalPrintNumber, List<string> OrderNumbers, IEnumerable<OrderPrintLogger> PrintLoggers) ReadExcelByOrderPrintLogger2PrintDate(DateTime? printDate, long fileId, Func<OrderPrintLogger,bool> func)
        {
            var excelList = ReadExcelByOrderPrintLogger(fileId).AsEnumerable();
            if (printDate.HasValue)
            {
                excelList = excelList.Where(p => p.PrintDate == printDate.Value);
            }

            if (func != default)
            {
                excelList = excelList.Where(func);
            }

            excelList = excelList.ToList();

            List<OrderPrintLogger> resultExeclList = new List<OrderPrintLogger>();
            foreach (var item in excelList)
            {
                if (!resultExeclList.Any(p=> p.ExpressNumber == item.ExpressNumber))
                {
                    resultExeclList.Add(item);
                }
            }

            return (resultExeclList.Count(),
                resultExeclList.SelectMany(p => string.IsNullOrWhiteSpace(p.OrderNumber) ? new string[0] : (p.OrderNumber.Split(",\n")))
                .Distinct().ToList(), resultExeclList);
        }
    }
}
