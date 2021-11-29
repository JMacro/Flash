using System.Threading.Tasks;

namespace Flash.Extensions.Resilience.Http
{
    public interface IHttpUrlResolver
    {
        Task<string> Resolve(string value);
    }
}