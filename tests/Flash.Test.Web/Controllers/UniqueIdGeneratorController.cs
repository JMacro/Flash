using Flash.Extensions.UidGenerator;
using Microsoft.AspNetCore.Mvc;

namespace Flash.Test.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniqueIdGeneratorController : ControllerBase
    {
        private readonly IUniqueIdGenerator _uniqueIdGenerator;

        public UniqueIdGeneratorController(IUniqueIdGenerator uniqueIdGenerator)
        {
            this._uniqueIdGenerator = uniqueIdGenerator;
        }

        [HttpGet("Test1")]
        public string Test1()
        {
            return this._uniqueIdGenerator.NewId().ToString(); 
        }
    }
}
