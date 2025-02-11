using Autofac;
using Flash.Extensions;
using Flash.Extensions.ChangeHistory;
using Flash.Test.StartupTests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flash.Test.EntityChange
{
    [TestFixture]
    public class EntityChangeTests : BaseTest<EntityChangeStartupTest>
    {
        [Test]
        public void CompareAsEqualTest()
        {
            var change = this.ServiceProvider.GetService<IEntityChange>();

            var id = Guid.NewGuid();
            var st1 = new Student()
            {
                ChangeObjectId = id,
                Id = 1,
                Age = 16,
                Name = null,
                Sex = EStudentSex.Male,
                CreateTime = DateTime.Now,
                Monery = 1,
                Lists = new List<ListObject> { new ListObject { Id = 1, Name = "1" } },
            };

            var result1 = change.Compare(id, st1, st1.DeepClone());
            Assert.IsNotNull(result1);
            Assert.That(result1.HistoryPropertys.Any(), Is.False);
        }

        [Test]
        public void CompareAsNotEqualTest()
        {
            var change = this.ServiceProvider.GetService<IEntityChange>();

            var id = Guid.NewGuid();
            var st1 = new Student()
            {
                ChangeObjectId = id,
                Id = 1,
                Age = 16,
                Name = null,
                Sex = EStudentSex.Male,
                CreateTime = DateTime.Now,
                Monery = 1,
                Lists = new List<ListObject> { new ListObject { Id = 1, Name = "1" } },
            };
            var st2 = new Student()
            {
                ChangeObjectId = Guid.NewGuid(),
                Id = 1,
                Age = 17,
                Name = "Test",
                Sex = null,
                CreateTime = st1.CreateTime.AddDays(-1),
                UpdateTime = DateTime.Now,
                Monery = 2,
                Lists = new List<ListObject> { new ListObject { Id = 1, Name = "2" } },
                TTT = 1
            };

            var result1 = change.Compare(id, st1, st2);
            Assert.IsNotNull(result1);
            Assert.That(result1.HistoryPropertys.Any(), Is.True);
        }

        [Test]
        public void CompareAsThrowTest()
        {
            var change = this.ServiceProvider.GetService<IEntityChange>();

            var id = Guid.NewGuid();
            var st1 = new Student()
            {
                ChangeObjectId = id,
                Id = 1,
                Age = 16,
                Name = null,
                Sex = EStudentSex.Male,
                CreateTime = DateTime.Now,
                Monery = 1,
                Lists = new List<ListObject> { new ListObject { Id = 1, Name = "1" } },
            };
            var st3 = new Student1()
            {
                ChangeObjectId = id,
                Id = 1,
                Age = 17,
                Name = "Test",
                Sex = null,
                CreateTime = st1.CreateTime.AddDays(-1),
                UpdateTime = DateTime.Now,
                Monery = 2,
                Lists = null,
                TTT = 1
            };

            Assert.That(new TestDelegate(() =>
            {
                var result1 = change.Compare(id, st1, st3);
            }), new ThrowsExceptionConstraint());
        }

        [Test]
        public void RecordTest()
        {
            var change = this.ServiceProvider.GetService<IEntityChange>();

            var id = Guid.NewGuid();
            var st1 = new Student()
            {
                ChangeObjectId = id,
                Id = 1,
                Age = 16,
                Name = null,
                Sex = EStudentSex.Male,
                CreateTime = DateTime.Now,
                Monery = 1,
                Lists = new List<ListObject> { new ListObject { Id = 1, Name = "1" } },
            };
            var st2 = new Student()
            {
                ChangeObjectId = Guid.NewGuid(),
                Id = 1,
                Age = 17,
                Name = "Test",
                Sex = null,
                CreateTime = st1.CreateTime.AddDays(-1),
                UpdateTime = DateTime.Now,
                Monery = 2,
                Lists = new List<ListObject> { new ListObject { Id = 1, Name = "2" } },
                TTT = 1
            };

            var result1 = change.Record(id, st1, st2).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(result1);
        }
    }

    public class TestStorage : IStorage
    {
        public Task<IBasePageResponse<ChangeHistoryInfo>> GetPageList(PageSearchQuery page)
        {
            return Task.FromResult(default(IBasePageResponse<ChangeHistoryInfo>));
        }

        public Task<bool> Insert(params ChangeHistoryInfo[] changes)
        {
            foreach (var item in changes)
            {
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(item));
            }
            return Task.FromResult(true);
        }
    }


    public class Student : IEntityChangeTracking
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EStudentSex? Sex { get; set; } = EStudentSex.Male;
        public int Age { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public double Monery { get; set; }
        public List<ListObject> Lists { get; set; } = new List<ListObject>();
        public int? TTT { get; set; }
        [IgnoreCheck]
        public object ChangeObjectId { get; set; }
    }

    public class Student1 : IEntityChangeTracking
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EStudentSex? Sex { get; set; } = EStudentSex.Male;
        public int Age { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public double Monery { get; set; }
        public List<ListObject> Lists { get; set; } = new List<ListObject>();
        public int? TTT { get; set; }
        public object ChangeObjectId { get; set; }
    }

    public class ListObject
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
    public enum EStudentSex
    {
        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Description("女")]
        Female = 1,
        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Description("男")]
        Male = 2,
    }
}
