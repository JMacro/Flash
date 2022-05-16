using Flash.Extensions.Resilience.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Flash.Test.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HttpTestController : ControllerBase
    {
        private readonly IHttpClient _httpClient;

        public HttpTestController(IHttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        [HttpGet("ResilientHttpTest")]
        public async Task<string> ResilientHttpTest()
        {
            var dd = await this._httpClient.PostAsync("http://192.168.18.241/api/Passport/login", new
            {
                username = "JMacro",
                password = "1",
                companyName = "捷扬讯科"
            });

            

            return await dd.ReadAsStringAsync();
        }
    }
}
