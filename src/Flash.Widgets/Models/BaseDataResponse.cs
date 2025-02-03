using Newtonsoft.Json;

namespace Flash.Widgets.Models
{
    public class BaseDataResponse<TData> : BaseResponse where TData : new()
    {
        /// <summary>
        /// 数据
        /// </summary>
        public TData Data { get; set; }

        public static BaseDataResponse<TData> Create(BaseResponse @base)
        {
            var result = new BaseDataResponse<TData>();
            result.ErrCode = @base.ErrCode;
            result.ErrMsg = @base.ErrMsg;
            return result;
        }

        public static BaseDataResponse<TData> Create(int errCode = -1, string errMsg = "", TData data = default(TData))
        {
            var result = new BaseDataResponse<TData>();
            result.ErrCode = errCode;
            result.ErrMsg = errMsg;
            result.Data = data;
            return result;
        }
    }
}
