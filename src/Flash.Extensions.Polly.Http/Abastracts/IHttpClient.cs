using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Polly.Http
{
    /// <summary>
    /// Http客户端
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// 获取Response响应内容（异步）
        /// </summary>
        /// <param name="uri">请求Uri</param>
        /// <param name="authorizationToken">Token值</param>
        /// <param name="authorizationType">认证类型</param>
        /// <param name="headers">请求头数据</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        Task<string> GetStringAsync(string uri, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// 获取Response响应内容
        /// </summary>
        /// <param name="uri">请求Uri</param>
        /// <param name="authorizationToken">Token值</param>
        /// <param name="authorizationType">认证类型</param>
        /// <param name="headers">请求头数据</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        string GetString(string uri, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Post操作（异步）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="uri">请求Uri</param>
        /// <param name="data">提交数据</param>
        /// <param name="authorizationToken">Token值</param>
        /// <param name="authorizationType">认证类型</param>
        /// <param name="headers">请求头数据</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        Task<HttpResponseMessage> PostAsync<T>(string uri, T data, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Post操作
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="uri">请求Uri</param>
        /// <param name="data">提交数据</param>
        /// <param name="authorizationToken">Token值</param>
        /// <param name="authorizationType">认证类型</param>
        /// <param name="headers">请求头数据</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        HttpResponseMessage Post<T>(string uri, T data, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Delete操作（异步）
        /// </summary>
        /// <param name="uri">请求Uri</param>
        /// <param name="authorizationToken">Token值</param>
        /// <param name="authorizationType">认证类型</param>
        /// <param name="headers">请求头数据</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Delete操作
        /// </summary>
        /// <param name="uri">请求Uri</param>
        /// <param name="authorizationToken">Token值</param>
        /// <param name="authorizationType">认证类型</param>
        /// <param name="headers">请求头数据</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        HttpResponseMessage Delete(string uri, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Put操作（异步）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="uri">请求Uri</param>
        /// <param name="data">提交数据</param>
        /// <param name="authorizationToken">Token值</param>
        /// <param name="authorizationType">认证类型</param>
        /// <param name="headers">请求头数据</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        Task<HttpResponseMessage> PutAsync<T>(string uri, T data, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Put操作
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="uri">请求Uri</param>
        /// <param name="data">提交数据</param>
        /// <param name="authorizationToken">Token值</param>
        /// <param name="authorizationType">认证类型</param>
        /// <param name="headers">请求头数据</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        HttpResponseMessage Put<T>(string uri, T data, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}
