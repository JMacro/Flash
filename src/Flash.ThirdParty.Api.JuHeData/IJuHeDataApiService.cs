using Flash.ThirdParty.Api.JuHeData.Model;
using System;
using System.Threading.Tasks;

namespace Flash.ThirdParty.Api.JuHeData
{
    public interface IJuHeDataApiService
    {
        /// <summary>
        /// 根据日期查询节假日安排等信息
        /// <para>https://www.juhe.cn/docs/api/id/606</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="date"></param>
        /// <param name="isShowDetail"></param>
        /// <returns></returns>
        Task<JuHeDataResult<CalendarInfoModel>> GetCalendarInfo(string key, DateTime date, bool isShowDetail = false);
        /// <summary>
        /// 各地出行防疫政策查询。查询全国高中低风险地区。
        /// <para>https://www.juhe.cn/docs/api/id/566</para>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<JuHeDataResult<SpringTravelRiskModel>> GetSpringTravelRisk(string key);
    }
}
