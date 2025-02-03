using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extensions.EventBus.Dashboard
{
    internal class CombinedResourceDispatcher : EmbeddedResourceDispatcher
    {
        private readonly Assembly _assembly;
        private readonly string _baseNamespace;
        private readonly string[] _resourceNames;

        public CombinedResourceDispatcher(
            [NotNull] string contentType,
            [NotNull] Assembly assembly,
            string baseNamespace,
            string[] resourceNames) : base(contentType, assembly, null)
        {
            _assembly = assembly;
            _baseNamespace = baseNamespace;
            _resourceNames = resourceNames;
        }

        protected override async Task WriteResponse(DashboardResponse response)
        {
            foreach (var resourceName in _resourceNames)
            {
                await WriteResource(
                    response,
                    _assembly,
                    $"{_baseNamespace}.{resourceName}").ConfigureAwait(false);
            }
        }
    }
}
