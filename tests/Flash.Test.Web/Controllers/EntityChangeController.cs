using Flash.Extensions.ChangeHistory;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flash.Test.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntityChangeController : ControllerBase
    {
        private readonly IEntityChange _entityChange;

        public EntityChangeController(IEntityChange entityChange)
        {
            this._entityChange = entityChange;
        }

        [HttpGet("test1")]
        public async Task<bool> Test1()
        {
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

            return await _entityChange.Record(st1, st2, st1.Id.ToString(), Guid.NewGuid().ToString(), "");
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
