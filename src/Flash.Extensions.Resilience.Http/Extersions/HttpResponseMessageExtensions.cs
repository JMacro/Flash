using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<string> ReadAsStringAsync(this HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> ReadAsStringAsync(this Task<HttpResponseMessage> httpResponseAsync)
        {
            var httpResponse = await httpResponseAsync.ConfigureAwait(false);
            return await httpResponse.ReadAsStringAsync();
        }

        public static async Task<TResponse> ReadAsObjectAsync<TResponse>(this HttpResponseMessage response)
        {
            var json = await response.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        public static async Task<TResponse> ReadAsObjectAsync<TResponse>(this Task<HttpResponseMessage> httpResponseAsync)
        {
            var httpResponse = await httpResponseAsync.ConfigureAwait(false);
            return await httpResponse.ReadAsObjectAsync<TResponse>();
        }
    }
}
