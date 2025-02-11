using Newtonsoft.Json;

namespace Flash.Widgets.Models
{
    public class BaseResponse
    {
        /// <summary>
        /// 返回码，非0表示失败,成功不会返回errcode
        /// </summary>
        public int ErrCode { get; set; } = 0;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess => this.ErrCode == 0;

        public static TData Create<TData>(int errCode = 0, string errMsg = "") where TData : BaseResponse, new()
        {
            var result = new TData();
            result.ErrCode = errCode;
            result.ErrMsg = errMsg;
            return result;
        }

        public static TData Create<TData>(BaseResponse @base) where TData : BaseResponse, new()
        {
            var result = new TData();
            result.ErrCode = @base.ErrCode;
            result.ErrMsg = @base.ErrMsg;
            return result;
        }

        public static BaseResponse Create(int errCode = 0, string errMsg = "")
        {
            var result = new BaseResponse();
            result.ErrCode = errCode;
            result.ErrMsg = errMsg;
            return result;
        }
    }
}
