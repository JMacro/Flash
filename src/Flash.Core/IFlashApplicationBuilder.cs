
using Microsoft.AspNetCore.Builder;

namespace Flash.Core
{
    public interface IFlashApplicationBuilder
    {
        IApplicationBuilder ApplicationBuilder { get; }
    }
}

