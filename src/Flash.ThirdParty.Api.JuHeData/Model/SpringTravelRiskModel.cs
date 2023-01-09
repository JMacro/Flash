using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Flash.ThirdParty.Api.JuHeData.Model
{
    public class SpringTravelRiskModel
    {
        /// <summary>
        /// 数据更新时间
        /// </summary>
        [JsonProperty("updated_date")]
        public DateTime UpdatedDate { get; set; }
        /// <summary>
        /// 高风险地区数量
        /// </summary>
        [JsonProperty("high_count")]
        public int HighCount { get; set; }
        /// <summary>
        /// 中风险地区数量
        /// </summary>
        [JsonProperty("middle_count")]
        public int MiddleCount { get; set; }
        /// <summary>
        /// 低风险地区数量
        /// </summary>
        [JsonProperty("low_count")]
        public int LowCount { get; set; }
        /// <summary>
        /// 高风险地区清单
        /// </summary>
        [JsonProperty("high_list")]
        public List<RiskAreaInfo> HighList { get; set; }
        /// <summary>
        /// 中风险地区清单
        /// </summary>
        [JsonProperty("middle_list")]
        public List<RiskAreaInfo> MiddleList { get; set; }
        /// <summary>
        /// 低风险地区清单
        /// </summary>
        [JsonProperty("low_list")]
        public List<RiskAreaInfo> LowList { get; set; }
    }

    public class RiskAreaInfo
    {
        /// <summary>
        /// 类型,1:全部区域 2:部分区域
        /// </summary>
        [JsonProperty("type")]
        public int AreaType { get; set; } = 0;
        /// <summary>
        /// 省份
        /// </summary>
        [JsonProperty("province")]
        public string Province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        [JsonProperty("county")]
        public string County { get; set; }
        /// <summary>
        /// 部分区域的详细列表,可能为null
        /// </summary>
        [JsonProperty("communitys")]
        public List<string> Communitys { get; set; }
        /// <summary>
        /// 地区的行政区划代码,可能为null
        /// </summary>
        [JsonProperty("county_code")]
        public string CountyCode { get; set; }
    }
}
