using Flash.DynamicRoute;
using Flash.Extensions.Tracting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Flash.Extensions.Resilience.Http
{
    public class StandardHttpClientFactory : IHttpClientFactory
    {
        private readonly ILogger<StandardHttpClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceLocator _serviceLocator;
        private readonly ITracerFactory _tracerFactory;

        public StandardHttpClientFactory(
            ILogger<StandardHttpClient> logger,
            IHttpContextAccessor httpContextAccessor,
            IServiceLocator serviceLocator,
            ITracerFactory tracerFactory)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _serviceLocator = serviceLocator;
            _tracerFactory = tracerFactory;
        }

        public IHttpClient CreateResilientHttpClient()
            => new StandardHttpClient(_logger, _httpContextAccessor, new HttpUrlResolver(_serviceLocator), _tracerFactory);
        public IHttpClient CreateResilientHttpClient(HttpMessageHandler httpMessageHandler)
        => new StandardHttpClient(_logger, _httpContextAccessor, new HttpUrlResolver(_serviceLocator), httpMessageHandler, _tracerFactory);



    }
}
