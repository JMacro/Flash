using Newtonsoft.Json;

namespace Flash.ThirdParty.Api.JuHeData.Model
{
    /// <summary>
    /// 聚合数据API接口结构
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class JuHeDataResult<TResult>
    {
        /// <summary>
        /// 状态描述
        /// </summary>
        [JsonProperty("reason")]
        public string Reason { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }
        /// <summary>
        /// 数据对象
        /// </summary>
        [JsonProperty("result")]
        public TResult Result { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess => ErrorCode == 0;
    }
}
