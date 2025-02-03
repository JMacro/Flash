using Flash.Extersions.OpenTracting;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extersions.CQRS.Implements
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        private readonly ILinkTrack _linkTrack;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ILinkTrack linkTrack)
        {
            _logger = logger;
            _linkTrack = linkTrack;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            using (var tracer = new Tracer(typeof(TRequest).Name))
            {
                tracer.SetTag("UserCode", _userInfoAnalysis.GetUserCode());
                if (request is IApiRequest)
                {
                    var requestData = request as IApiRequest;
                    if (string.IsNullOrEmpty(requestData.ClientIPAddress))
                    {
                        requestData.ClientIPAddress = this._httpContext.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                    }
                    tracer.LogRequest(requestData);
                }
                else
                {
                    tracer.LogRequest(request);
                }
                var response = await next();
                tracer.LogResponse(response);
                return response;
            }
        }
    }
}
