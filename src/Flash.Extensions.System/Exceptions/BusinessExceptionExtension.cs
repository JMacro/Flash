using System;

namespace Flash.Extensions
{
    /// <summary>
    /// 业务异常抛出扩展
    /// </summary>
    public static class BusinessExceptionExtension
    {
        /// <summary>
        /// 验证实体是否为null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">数据源</param>
        /// <param name="throwMessage">错误信息</param>
        /// <returns>当前数据源</returns>
        /// <exception cref="BusinessException"></exception>
        public static T ThrowIfNull<T>(this T source, string throwMessage = "非法请求") where T : class
        {
            if (source == null) BusinessException.Throw(throwMessage);
            return source;
        }

        /// <summary>
        /// 当满足表达式条件后将抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">数据源</param>
        /// <param name="expression">表达式</param>
        /// <param name="throwMessage">错误信息</param>
        /// <returns>当前数据源</returns>
        /// <exception cref="BusinessException"></exception>
        public static T ThrowIf<T>(this T source, Predicate<T> expression, string throwMessage = "非法请求") where T : class
        {
            if (expression(source)) BusinessException.Throw(throwMessage);
            return source;
        }
    }
}
