namespace System
{
    public static class UriExtensions
    {
        /// <summary>
        /// 是否为Https请求
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsHttps(this Uri address)
        {
            var url = address.ToString().ToLower();
            return url.IndexOf("https") >= 0;
        }
    }
}
