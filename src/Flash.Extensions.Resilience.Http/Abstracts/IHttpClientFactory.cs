using System.Net.Http;

namespace Flash.Extensions.Resilience.Http
{
    public interface IHttpClientFactory
    {
        IHttpClient CreateResilientHttpClient();

        IHttpClient CreateResilientHttpClient(HttpMessageHandler httpMessageHandler);
    }
}