using System;
using System.Net;
using System.Threading.Tasks;

namespace Flash.Extensions.EventBus.Dashboard
{
    internal class BatchCommandDispatcher : IDashboardDispatcher
    {
        private readonly Action<DashboardContext, string> _command;

        public BatchCommandDispatcher(Action<DashboardContext, string> command)
        {
            _command = command;
        }

        public async Task Dispatch(DashboardContext context)
        {
            if (context.IsReadOnly)
            {
                context.Response.StatusCode = 401;
                return;
            }

            var jobIds = await context.Request.GetFormValuesAsync("jobs[]").ConfigureAwait(false);
            if (jobIds.Count == 0)
            {
                context.Response.StatusCode = 422;
                return;
            }

            foreach (var jobId in jobIds)
            {
                _command(context, jobId);
            }

            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
        }
    }
}
