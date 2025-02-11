using Flash.Extensions.Tracting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Wrap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Resilience.Http
{
    /// <summary>
    /// 弹性HTTP连接客户端，集成重试和熔断策略
    /// </summary>
    public class ResilientHttpClient : IHttpClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ResilientHttpClient> _logger;
        private readonly Func<string, IEnumerable<IAsyncPolicy>> _policyCreator;
        private readonly ConcurrentDictionary<string, AsyncPolicyWrap> _policyWrappers;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _compomentName = typeof(ResilientHttpClient).FullName;
        private readonly IHttpUrlResolver _httpUrlResolver;
        private ITracerFactory _tracerFactory = null;

        public ResilientHttpClient(
            Func<string, IEnumerable<IAsyncPolicy>> policyCreator,
            ILogger<ResilientHttpClient> logger,
            IHttpContextAccessor httpContextAccessor,
            IHttpUrlResolver httpUrlResolver,
            ITracerFactory tracerFactory)
        {

            _client = new HttpClient();
            _logger = logger;
            _policyCreator = policyCreator;
            _httpUrlResolver = httpUrlResolver;
            _policyWrappers = new ConcurrentDictionary<string, AsyncPolicyWrap>();
            _httpContextAccessor = httpContextAccessor;
            _tracerFactory = tracerFactory;
        }

        public ResilientHttpClient(
        Func<string, IEnumerable<IAsyncPolicy>> policyCreator,
        ILogger<ResilientHttpClient> logger,
        IHttpContextAccessor httpContextAccessor,
        IHttpUrlResolver httpUrlResolver,
        HttpMessageHandler httpMessageHandler,
        ITracerFactory tracerFactory)
        {

            _client = new HttpClient(httpMessageHandler);
            _logger = logger;
            _policyCreator = policyCreator;
            _httpUrlResolver = httpUrlResolver;
            _policyWrappers = new ConcurrentDictionary<string, AsyncPolicyWrap>();
            _httpContextAccessor = httpContextAccessor;
            _tracerFactory = tracerFactory;
        }


        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string authorizationMethod = "Bearer", IDictionary<string, string> dictionary = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PostAsync(uri, item, HttpMediaType.Json, authorizationToken, authorizationMethod, dictionary, cancellationToken);
        }

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item, HttpMediaType httpMediaType, string authorizationToken = null, string authorizationMethod = "Bearer", IDictionary<string, string> dictionary = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return DoPostPutAsync(HttpMethod.Post, uri, item, httpMediaType, authorizationToken, authorizationMethod, dictionary, cancellationToken);
        }

        public Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string authorizationMethod = "Bearer", IDictionary<string, string> dictionary = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PutAsync(uri, item, HttpMediaType.Json, authorizationToken, authorizationMethod, dictionary, cancellationToken);
        }

        public Task<HttpResponseMessage> PutAsync<T>(string uri, T item, HttpMediaType httpMediaType, string authorizationToken = null, string authorizationMethod = "Bearer", IDictionary<string, string> dictionary = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return DoPostPutAsync(HttpMethod.Put, uri, item, httpMediaType, authorizationToken, authorizationMethod, dictionary, cancellationToken);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer", IDictionary<string, string> dictionary = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            uri = await ResolveUri(uri);
            var origin = GetOriginFromUri(uri);

            return await HttpInvoker(origin, async (context, ctx) =>
           {
               using (var tracer = _tracerFactory.CreateTracer("HTTP DELETE"))
               {
                   tracer.SetTag("http.url", uri);
                   tracer.SetTag("http.method", "DELETE");

                   var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

                   SetAuthorizationHeader(requestMessage);

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

                   #region LOG:记录返回
                   var responseContent = await response.Content.ReadAsStringAsync();
                   tracer.SetTag("http.status_code", (int)response.StatusCode);

                   if (dictionary != null && dictionary.ContainsKey("x-masking") && (dictionary["x-masking"] == "all" || dictionary["x-masking"] == "response"))
                   {
                       //日志脱敏不记录
                   }
                   else
                   {
                       _logger.LogInformation("Http Request Executed :{responseContent}", responseContent);
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

        public async Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer", IDictionary<string, string> dictionary = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            uri = await ResolveUri(uri);
            var origin = GetOriginFromUri(uri);

            return await HttpInvoker(origin, async (context, ctx) =>
            {
                using (var tracer = _tracerFactory.CreateTracer("HTTP GET"))
                {
                    tracer.SetTag("http.url", uri);
                    tracer.SetTag("http.method", "GET");
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                    SetAuthorizationHeader(requestMessage);

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

                    #region LOG：记录返回
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

                    return responseContent;
                }
            }, cancellationToken);
        }

        private async Task<HttpResponseMessage> DoPostPutAsync<T>(HttpMethod method, string uri, T item, HttpMediaType httpMediaType = HttpMediaType.Json, string authorizationToken = null, string authorizationMethod = "Bearer", IDictionary<string, string> dictionary = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            }

            uri = await ResolveUri(uri);
            var origin = GetOriginFromUri(uri);

            return await HttpInvoker(origin, async (context, ctx) =>
            {
                using (var tracer = _tracerFactory.CreateTracer($"HTTP {method.Method.ToUpper()}"))
                {
                    tracer.SetTag("http.url", uri);
                    tracer.SetTag("http.method", method.Method.ToUpper());

                    var requestMessage = new HttpRequestMessage(method, uri);
                    var requestContent = JsonConvert.SerializeObject(item);
                    switch (httpMediaType)
                    {
                        case HttpMediaType.FormData:
                            requestContent = JsonConvert.SerializeObject(item);
                            var dic = item.ToFormData();
                            var formDataContent = new MultipartFormDataContent();
                            foreach (var kv in dic)
                            {
                                formDataContent.Add(new StringContent(kv.Value, System.Text.Encoding.UTF8), string.Format("\"{0}\"", kv.Key));
                            }
                            requestMessage.Content = formDataContent;
                            break;
                        case HttpMediaType.Json:
                        default:
                            requestMessage.Content = new StringContent(requestContent, System.Text.Encoding.UTF8, "application/json");
                            break;
                    }

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

        private async Task<T> HttpInvoker<T>(string origin, Func<Context, CancellationToken, Task<T>> action, CancellationToken cancellationToken)
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
        private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authorizationHeader))
                {
                    requestMessage.Headers.Add("Authorization", new List<string>() { authorizationHeader });
                }
            }
        }
    }
}
