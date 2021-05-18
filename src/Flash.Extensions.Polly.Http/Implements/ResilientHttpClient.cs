using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extersions.Polly.Http
{
    public class ResilientHttpClient : IHttpClient
    {
        private readonly ILogger<ResilientHttpClient> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Func<string, IEnumerable<IAsyncPolicy>> _policyCreator;
        private readonly ConcurrentDictionary<string, AsyncPolicyWrap> _policyWrappers;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResilientHttpClient(
            ILogger<ResilientHttpClient> logger,
            IHttpClientFactory httpClientFactory,
            Func<string, IEnumerable<IAsyncPolicy>> policyCreator,
            IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._httpClientFactory = httpClientFactory;
            this._policyCreator = policyCreator;
            this._policyWrappers = new ConcurrentDictionary<string, AsyncPolicyWrap>();
            this._httpContextAccessor = httpContextAccessor;
        }

        public HttpResponseMessage Delete(string uri, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public string GetString(string uri, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetStringAsync(string uri, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Post<T>(string uri, T data, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T data, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            return DoPostPutAsync(HttpMethod.Post, uri, data, authorizationToken, authorizationType, headers, cancellationToken);
        }

        public HttpResponseMessage Put<T>(string uri, T data, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> PutAsync<T>(string uri, T data, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private async Task<HttpResponseMessage> DoPostPutAsync<T>(HttpMethod method, string uri, T item, string authorizationToken = null, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Bearer, IDictionary<string, string> dictionary = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            }

            uri = await ResolveUri(uri);
            var origin = GetOriginFromUri(uri);

            return await HttpInvokerAsync(origin, async (context, ctx) =>
            {
                using (var tracer = new Hummingbird.Extensions.Tracing.Tracer($"HTTP {method.Method.ToUpper()}"))
                {
                    tracer.SetComponent(_compomentName);
                    tracer.SetTag("http.url", uri);
                    tracer.SetTag("http.method", method.Method.ToUpper());

                    var requestMessage = new HttpRequestMessage(method, uri);
                    var requestContent = JsonConvert.SerializeObject(item);

                    #region LOG：记录请求
                    if (dictionary != null && dictionary.ContainsKey("x-masking") && (dictionary["x-masking"] == "all" || dictionary["x-masking"] == "request"))
                    {
                        //日志脱敏                           
                    }
                    else
                    {
                        _logger.LogInformation("Http Request Executing:{requestContent}", requestContent);
                    }
                    #endregion

                    SetAuthorizationHeader(requestMessage);

                    requestMessage.Content = new StringContent(requestContent, System.Text.Encoding.UTF8, "application/json");

                    if (authorizationToken != null)
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                    }

                    if (dictionary != null)
                    {
                        foreach (var key in dictionary.Keys)
                        {
                            requestMessage.Headers.Add(key, dictionary[key]);
                        }
                    }

                    var response = await _client.SendAsync(requestMessage, ctx);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    #region LOG:记录返回
                    tracer.SetTag("http.status_code", (int)response.StatusCode);

                    if (dictionary != null && dictionary.ContainsKey("x-masking") && (dictionary["x-masking"] == "all" || dictionary["x-masking"] == "response"))
                    {
                        //日志脱敏不记录
                    }
                    else
                    {
                        _logger.LogInformation("Http Request Executed:{responseContent}", responseContent);
                    }
                    #endregion

                    if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        throw new HttpRequestException(response.ReasonPhrase);
                    }

                    return response;

                }

            }, cancellationToken);
        }

        private async Task<string> ResolveUri(string uri)
        {
            return await _httpUrlResolver.Resolve(uri);
        }

        private async Task<T> HttpInvokerAsync<T>(string origin, Func<Context, CancellationToken, Task<T>> action, CancellationToken cancellationToken)
        {
            var normalizedOrigin = NormalizeOrigin(origin);

            if (!_policyWrappers.TryGetValue(normalizedOrigin, out AsyncPolicyWrap policyWrap))
            {
                policyWrap = Policy.WrapAsync(_policyCreator(normalizedOrigin).ToArray());
                _policyWrappers.TryAdd(normalizedOrigin, policyWrap);
            }

            return await policyWrap.ExecuteAsync(action, new Context(normalizedOrigin), cancellationToken);
        }

        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }

        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);
            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";

            return origin;
        }
    }
}
