using System;
namespace Flash.Extensions.OpenTracting
{
	public interface IRegisterResponseTracrIdService
	{
		/// <summary>
		/// 获得跟踪Id
		/// </summary>
		/// <returns></returns>
		string GetTraceId();
	}
}

