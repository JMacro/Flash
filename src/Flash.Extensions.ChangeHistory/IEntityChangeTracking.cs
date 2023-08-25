using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Flash.Extensions.ChangeHistory
{
    public interface IEntityChangeTracking
    {
        /// <summary>
        /// 变更对象Id
        /// </summary>
        [IgnoreCheck, NotMapped, JsonIgnore]
        object ChangeObjectId { get; set; }
    }
}
