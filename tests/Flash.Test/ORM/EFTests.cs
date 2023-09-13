using Flash.Extensions;
using Flash.Extensions.ORM;
using Flash.Test.ORM.Base;
using Flash.Test.StartupTests;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Flash.Test.ORM
{
    [TestFixture]
    public class EFTests : BaseTest<EFStartupTest>
    {
        [Test]
        public void WhereLikeTest()
        {
            var testDbContext = this.ServiceProvider.GetService<TestDb1Context>();
            var query = new
            {
                CName = "系"
            };
            var result = testDbContext.Set<AccountInfo>().WhereWith(query, l => l.CName, r => r.CName, OperatorType.Like).ToList();
            Assert.IsNotNull(result);
        }

        [Test]
        public void WhereLeftLikeTest()
        {
            var testDbContext = this.ServiceProvider.GetService<TestDb1Context>();
            var query = new
            {
                CName = "%"
            };
            var result = testDbContext.Set<AccountInfo>().WhereWith(query, l => l.CName, r => r.CName, OperatorType.LeftLike).ToList();
            Assert.IsNotNull(result);
        }

        [Test]
        public void WhereRightLikeTest()
        {
            var testDbContext = this.ServiceProvider.GetService<TestDb1Context>();
            var query = new
            {
                CName = "系"
            };
            var result = testDbContext.Set<AccountInfo>().WhereWith(query, l => l.CName, r => r.CName, OperatorType.RightLike).ToList();
            Assert.IsNotNull(result);
        }

        [Test]
        public void QueryPageTest()
        {
            var testDbContext = this.ServiceProvider.GetService<TestDb1Context>();
            var page = testDbContext.Set<AccountInfo>().QueryPageAsync(new PageQuery() { }, (e, p) =>
                p.Add(OrderBy.Create(e, s => s.Id, PageOrderBy.ASC))
                .Add(OrderBy.Create(e, s => s.EName, PageOrderBy.DESC))
                .Add(e, s => s.Account, PageOrderBy.ASC)
                ).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsNotNull(page);
        }

        [Test]
        public void UpdateTrackingTest()
        {
            var testDbContext = this.ServiceProvider.GetService<TestDb1Context>();
            var result = testDbContext.Set<AccountInfo>().FirstOrDefault();
            result.ModifiedTime = DateTime.Now;
            result.CreateName = Guid.NewGuid().ToString();
            testDbContext.SaveChanges();
            Assert.IsNotNull(result);

            var result1 = testDbContext.Set<AccountInfo>().FirstOrDefault();
            result1.ModifiedTime = DateTime.Now;
            testDbContext.SaveChanges();
            Assert.IsNotNull(result1);
        }

        [Test]
        public void GetRepositoryTest()
        {
            var repository = this.ServiceProvider.GetService<IRepository<AccountInfo>>();
            Assert.IsNotNull(repository);
            var data = repository.GetById((long)0);


            var test1Repository = this.ServiceProvider.GetService<ITest1Repository>();
            Assert.IsNotNull(test1Repository);

            data = test1Repository.GetById((long)0);


            var test2Repository = this.ServiceProvider.GetService<ITest2Repository>();
            Assert.IsNotNull(test2Repository);

            data = test2Repository.GetById((long)0);

        }
    }
}
