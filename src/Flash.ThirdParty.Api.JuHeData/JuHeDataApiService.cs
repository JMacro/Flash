using Flash.Extensions;
using Flash.Extensions.Resilience.Http;
using Flash.ThirdParty.Api.JuHeData.Model;
using System;
using System.Threading.Tasks;

namespace Flash.ThirdParty.Api.JuHeData
{
    public class JuHeDataApiService : IJuHeDataApiService
    {
        private readonly IHttpClient _httpClient;

        public JuHeDataApiService(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JuHeDataResult<CalendarInfoModel>> GetCalendarInfo(string key, DateTime date, bool isShowDetail = false)
        {
            var result = await _httpClient.GetStringAsync($"http://apis.juhe.cn/fapig/calendar/day.php?key={key}&date={date.ToString("yyyy-MM-dd")}&detail={(isShowDetail ? 1 : 0)}");
            return result.ToObject<JuHeDataResult<CalendarInfoModel>>();
        }

        public async Task<JuHeDataResult<SpringTravelRiskModel>> GetSpringTravelRisk(string key)
        {
            var result = await _httpClient.GetStringAsync($"http://apis.juhe.cn/springTravel/risk?key={key}");
            return result.ToObject<JuHeDataResult<SpringTravelRiskModel>>();
        }
    }
}
