using System;
using System.ComponentModel.DataAnnotations;

namespace Flash.Extensions.ORM
{
    public class BaseEntity<TKeyType> : IEntity<TKeyType> where TKeyType : struct
    {
        [Key]
        public TKeyType Id { get; set; }
        public DateTime CreateTime { get; set; }
        public TKeyType CreateUserId { get; set; }
        public DateTime LastModifyTime { get; set; }
        public TKeyType LastModifyUserId { get; set; }
        public bool IsDelete { get; set; }
    }
}
