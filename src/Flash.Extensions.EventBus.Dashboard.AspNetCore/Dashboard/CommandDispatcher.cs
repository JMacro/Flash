using System;
using System.Net;
using System.Threading.Tasks;

namespace Flash.Extensions.EventBus.Dashboard
{
    internal class CommandDispatcher : IDashboardDispatcher
    {
        private readonly Func<DashboardContext, bool> _command;

        public CommandDispatcher(Func<DashboardContext, bool> command)
        {
            _command = command;
        }


        public Task Dispatch(DashboardContext context)
        {
            if (context.IsReadOnly)
            {
                context.Response.StatusCode = 401;
                return Task.FromResult(false);
            }

            var request = context.Request;
            var response = context.Response;

            if (!"POST".Equals(request.Method, StringComparison.OrdinalIgnoreCase))
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return Task.FromResult(false);
            }

            if (_command(context))
            {
                response.StatusCode = (int)HttpStatusCode.NoContent;
            }
            else
            {
                response.StatusCode = 422;
            }

            return Task.FromResult(true);
        }
    }
}
