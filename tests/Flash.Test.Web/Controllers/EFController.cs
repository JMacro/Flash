using Flash.Extensions;
using Flash.Extensions.ORM;
using Flash.Test.Web.EFCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flash.Test.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EFController : ControllerBase
    {
        private readonly TestDbContext _testDbContext;

        public EFController(TestDbContext testDbContext)
        {
            this._testDbContext = testDbContext;
        }

        [HttpGet("test1")]
        public async Task<string> Test1(string q)
        {
            var query = new
            {
                CName = q
            };
            var df = this._testDbContext.Set<AccountInfo>().WhereWith(query, l => l.CName, r => r.CName, OperatorType.Like).ToList();

            return "";
        }

        [HttpGet("test2")]
        public async Task<string> Test2(string q)
        {
            var query = new
            {
                CName = q
            };
            var df = this._testDbContext.Set<AccountInfo>().WhereWith(query, l => l.CName, r => r.CName, OperatorType.LeftLike).ToList();

            return "";
        }

        [HttpGet("test3")]
        public async Task<string> Test3(string q)
        {
            var query = new
            {
                CName = q
            };
            var df = this._testDbContext.Set<AccountInfo>().WhereWith(query, l => l.CName, r => r.CName, OperatorType.RightLike).ToList();

            return "";
        }

        [HttpGet("test4")]
        public async Task<object> Test4(string q)
        {
            var query = new
            {
                CName = q
            };

            var dff = this._testDbContext.Set<AccountInfo>().WhereWith(query, l => l.CName, r => r.CName, OperatorType.RightLike).ToList();

            await this._testDbContext.Set<AccountInfo>().QueryPageAsync(new PageQuery() { }, p => p.Account, false);

            return await this._testDbContext.Set<AccountInfo>().QueryPageAsync(new PageQuery() { }, (e, p) =>
                p.Add(OrderBy.Create(e, s => s.Id, PageOrderBy.ASC))
                .Add(OrderBy.Create(e, s => s.EName, PageOrderBy.DESC))
                .Add(e, s => s.Account, PageOrderBy.ASC)
                );
        }
    }
}
