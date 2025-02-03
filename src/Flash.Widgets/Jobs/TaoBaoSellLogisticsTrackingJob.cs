using System;
using System.Security.Cryptography;
using System.Text;
using Flash.Extensions;
using Flash.Extensions.Job;
using Flash.Extensions.Job.Hangfire;
using Flash.Extensions.ORM;
using Flash.Extensions.Resilience.Http;
using Flash.Widgets.Configures;
using Flash.Widgets.DbContexts;
using Flash.Widgets.Models.TaobaoUtils;
using Hangfire.Server;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Flash.Widgets.Jobs
{
	public class TaoBaoSellLogisticsTrackingJob : BaseHangfireJob
    {
        private readonly TaoBaoDbContext _dbContext;
        private readonly IHttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IOptionsMonitor<ExpressCodeConfigure> _optionsMonitor;

        public TaoBaoSellLogisticsTrackingJob(
            ILogger<TaoBaoSellLogisticsTrackingJob> logger,
            TaoBaoDbContext dbContext,
            IHttpClient httpClient,
            IConfiguration configuration,
            IOptionsMonitor<ExpressCodeConfigure> optionsMonitor) : base(logger)
        {
            this._dbContext = dbContext;
            this._httpClient = httpClient;
            this._configuration = configuration;
            this._optionsMonitor = optionsMonitor;
        }

        public override async Task Execute(IJobExecutionContextContainer<PerformContext> contextContainer)
        {
            var requestData = new SellOrder2LogisticsTrackingGetListRequestData();
            requestData.PageSize = 50;

            IBasePageResponse<LogisticsTrackingEntity> resultPage = null;
            do
            {
                var queryable = this._dbContext.Set<LogisticsTrackingEntity>().AsQueryable().Where(p => !p.IsDelete && (p.State == LogisticsTrackingState.WaitHandle || p.State == LogisticsTrackingState.SystemTracking) && !string.IsNullOrWhiteSpace(p.ExpressNumber));
                resultPage = await queryable.QueryPageAsync(requestData, (t, c) => c.Add(OrderBy.Create(t, s => s.State, PageOrderBy.ASC)));

                if (resultPage.HasData)
                {
                    foreach (var info in resultPage.List)
                    {
                        var expressCodeInfo = this._optionsMonitor.CurrentValue.ExpressCodes.FirstOrDefault(p => p.ExpressName == info.ExpressName);
                        if (expressCodeInfo == null)
                        {
                            continue;
                        }

                        var request = new
                        {
                            com = expressCodeInfo.KuaiDi100Code,
                            num = info.ExpressNumber.Replace("No:", ""),
                        };
                        var param = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                        var customer = this._configuration["KuaiDi100:Customer"];
                        var sign = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes($"{param}{this._configuration["KuaiDi100:Key"]}{customer}")));//param+key+customer

                        var result = await this._httpClient.PostAsync($"https://poll.kuaidi100.com/poll/query.do?customer={customer}&sign={sign}&param={param}", new { });
                        var trackingResult = await result.ReadAsStringAsync();
                        if (!string.IsNullOrWhiteSpace(trackingResult))
                        {
                            var jObject = Newtonsoft.Json.Linq.JObject.Parse(trackingResult);

                            if ((jObject.ContainsKey("returnCode") && jObject.Value<int>("returnCode") != 200) || ( jObject.ContainsKey("status") && jObject.Value<int>("status") != 200))
                            {
                                //5次重试
                                info.RetryCount++;
                                if (info.RetryCount >= 6)
                                {
                                    info.State = LogisticsTrackingState.Error;
                                    info.SystemRemarks = "请求物流查询系统异常";
                                }
                            }
                            else
                            {
                                var firstTrackInfo = jObject.Value<JArray>("data")?.FirstOrDefault();
                                var firstContext = "";
                                DateTime? lastTime = null;
                                if (firstTrackInfo != null)
                                {
                                    firstContext = firstTrackInfo.Value<string>("context");
                                    lastTime = firstTrackInfo.Value<DateTime>("time");
                                }
                                
                                if (info.State == LogisticsTrackingState.SystemTracking)
                                {
                                    if (info.BusinessTime != lastTime)
                                    {
                                        info.RetryCount = 0;
                                    }
                                }
                                else
                                {
                                    info.State = LogisticsTrackingState.SystemTracking;
                                }

                                info.BusinessTime = lastTime;
                                switch (jObject.Value<int>("state"))
                                {
                                    case 3:
                                    case 301:
                                    case 302:
                                    case 303:
                                    case 304:
                                        info.State = LogisticsTrackingState.CompletedSign;
                                        info.SystemRemarks = $"已签收 -> {firstContext}";
                                        break;
                                    case 4:
                                    case 401:
                                    case 14:
                                        info.State = LogisticsTrackingState.CompletedSign;
                                        info.SystemRemarks = $"已退回签收 -> {firstContext}";
                                        break;
                                    default:
                                        info.RetryCount++;
                                        if (info.RetryCount >= 6)
                                        {
                                            info.State = LogisticsTrackingState.Stagnation;
                                            info.SystemRemarks = $"物流轨迹长时间未更新 -> {firstContext}";
                                        }
                                        break;
                                }
                            }
                            info.LogisticsTracking = trackingResult;
                            this._dbContext.SaveChanges();
                        }
                    }

                    requestData.PageIndex++;
                }
            } while (resultPage.HasData);

            
        }
    }
}

