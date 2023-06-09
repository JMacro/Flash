using Flash.Extensions;
using Flash.Extensions.ChangeHistory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Test
{
    [TestClass]
    public class EntityChangeTest
    {
        [TestMethod]
        public void TestChange()
        {
            var change = new EntityChange(new TestStorage());

            var st1 = new Student()
            {
                Id = 1,
                Age = 16,
                Name = null,
                Sex = EStudentSex.Male,
                CreateTime = DateTime.Now,
                Monery = 1,
            };
            var st2 = new Student()
            {
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

            change.Record(st1, st2, st1.Id.ToString(), Guid.NewGuid().ToString(), "").ConfigureAwait(false).GetAwaiter().GetResult();

            var result = change.GetPageList(new PageSearchQuery { }).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }

    public class TestStorage : IStorage
    {
        public Task<IBasePageResponse<ChangeHistoryInfo>> GetPageList(PageSearchQuery page)
        {
            throw new NotImplementedException();
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


    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EStudentSex? Sex { get; set; } = EStudentSex.Male;
        public int Age { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public double Monery { get; set; }
        [IgnoreCheck]
        public List<string> Lists { get; set; } = new List<string>();
        public int? TTT { get; set; }
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
