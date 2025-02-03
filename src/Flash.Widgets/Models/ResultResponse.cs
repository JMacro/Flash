using System;
namespace Flash.Widgets.Models
{
	public class ResultResponse<T> : ResultResponse where T : class, new()
	{
		public T Data { get; set; }

		public static ResultResponse<T> Create<T>(int code,T data) where T : class, new()
        {
			return new ResultResponse<T>
			{
				Code = code,
				Data = data
			};
		}
	}

    public class ResultResponse
    {
        public string Msg { get; set; }
        public int Code { get; set; }
        public bool Result => Code >= 0;

        public static ResultResponse Create(int code,string msg)
        {
            return new ResultResponse
            {
                Code = code,
                Msg = msg
            };
        }
    }
}

