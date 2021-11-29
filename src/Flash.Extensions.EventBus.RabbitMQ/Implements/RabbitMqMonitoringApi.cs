using AutoMapper;
using Flash.Extensions.Resilience.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Flash.Extensions.EventBus.RabbitMQ
{
    public sealed class RabbitMqMonitoringApi : IMonitoringApi
    {
        private readonly IHttpClient _httpClient;
        private readonly RabbitMQOption _option;
        private readonly IMapper _mapper;

        public RabbitMqMonitoringApi(IHttpClient httpClient, RabbitMQOption option, IMapper mapper)
        {
            this._httpClient = httpClient;
            this._option = option;
            this._mapper = mapper;
        }

        public async Task<List<QueueWithAllEnqueuedDto>> GetQueues()
        {
            var responseResult = await this._httpClient.GetStringAsync(GetRequestUrl($"/api/queues/{HttpUtility.UrlEncode(this._option.VirtualHost)}"), GetAuthorizationToken(), "Basic");
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RabbitMqQueueWithAllEnqueuedDto>>(responseResult);
            return this._mapper.Map<List<QueueWithAllEnqueuedDto>>(result);
        }

        public async Task<List<QueueWithAllEnqueuedDto>> GetFailedQueues()
        {
            var list = await GetQueues();
            return list.Where(p => p.MessageCount > 0 && p.QueueName.Contains("@Failed")).ToList();
        }

        public async Task<List<QueueWithAllEnqueuedDto>> GetRetryQueues()
        {
            var list = await GetQueues();
            return list.Where(p => p.MessageCount > 0 && p.QueueName.Contains("@Delay")).ToList();
        }

        public async Task<List<QueueWithAllEnqueuedDto>> GetNormalQueues()
        {
            var regex = new Regex("^[a-zA-Z0-9_-]+$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var list = await GetQueues();
            return list.Where(p => p.MessageCount > 0 && regex.Match(p.QueueName).Success).ToList();
        }

        public int GetFailedCount()
        {
            var result = GetFailedQueues().ConfigureAwait(false).GetAwaiter().GetResult();
            return result.Count;
        }

        public int GetRetryCount()
        {
            var result = GetRetryQueues().ConfigureAwait(false).GetAwaiter().GetResult();
            return result.Count;
        }

        //
        public int GetNormalCount()
        {
            var result = GetNormalQueues().ConfigureAwait(false).GetAwaiter().GetResult();
            return result.Count;
        }

        public async Task<List<QueueMessageDto>> GetMessages(string queueName)
        {
            //object count = request.TryGetValue("count", out count) ? count : 1;

            if (queueName == null || string.IsNullOrWhiteSpace(queueName.ToString()))
            {
                throw new ArgumentNullException("Undefined parameter queueName or parameter queueName is null");
            }

            var httpResponse = await this._httpClient.PostAsync<object>(GetRequestUrl($"/api/queues/{HttpUtility.UrlEncode(this._option.VirtualHost)}/{HttpUtility.UrlEncode(queueName.ToString())}/get"), new
            {
                vhost = this._option.VirtualHost,
                name = queueName,
                truncate = 50000,
                ackmode = "ack_requeue_true",
                encoding = "auto",
                count = 1000
            }, GetAuthorizationToken(), "Basic");

            var responseResult = await httpResponse.Content.ReadAsStringAsync();

            var result = new List<QueueMessageDto>();
            var array = (JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(responseResult);
            foreach (var item in array)
            {
                var obj = new QueueMessageDto();
                obj.Message = item["payload"].ToString();
                obj.MessageId = item["properties"]["message_id"].ToString();
                obj.NormalQueueName = item["properties"]["headers"]["x-dead-letter-routing-key"].ToString();
                result.Add(obj);
            }

            return result;
        }

        /// <summary>
        /// 获得请求URL
        /// </summary>
        /// <param name="path">api路径</param>
        /// <returns></returns>
        private string GetRequestUrl(string path) => $"{this._option.DashboardProtocol}://{this._option.HostName}:{this._option.DashboardPort}/{path.TrimStart('/')}";

        /// <summary>
        /// 获得授权Token
        /// </summary>
        /// <returns></returns>
        private string GetAuthorizationToken() => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{this._option.UserName}:{this._option.Password}"));
    }
}
