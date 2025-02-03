using Newtonsoft.Json;

namespace Flash.ThirdParty.Api.JuHeData.Model
{
    public class CalendarInfoModel
    {
        /// <summary>
        /// 阳历日期
        /// </summary>
        [JsonProperty("date")]
        public string Date { get; set; }
        /// <summary>
        /// 表示星期几
        /// </summary>
        [JsonProperty("week")]
        public string Week { get; set; }
        /// <summary>
        /// 状态描述，节假日/工作日/周末
        /// </summary>
        [JsonProperty("statusDesc")]
        public string StatusDesc { get; set; }
        /// <summary>
        /// 当天状态标识，1:节假日，2:工作日
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
