using Flash.AspNetCore.WorkFlow.Domain.Events;
using Flash.AspNetCore.WorkFlow.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.FormTypes
{
    /// <summary>
    /// 表单类型
    /// </summary>
    internal partial class FormType : AggregateRoot
    {
        public string Name { get; private set; }
        public IDictionary<string, string> Fields { get; private set; }

        public long Id { get; private set; }
        public bool IsDelete { get; private set; }
        public DateTime CreateTime { get; private set; }
        public long CreateUserId { get; private set; }
        public DateTime LastModifyTime { get; private set; }
        public long LastModifyUserId { get; private set; }

        public void Init(string name, IDictionary<string, string> fields)
        {
            var @event = new FormTypeInitEvent
            {
                Id = this._uniqueIdGenerator.NewId(),
                AggregateId = this._uniqueIdGenerator.NewId(),
                Fields = fields,
                Name = name,
                Version = -1
            };

            Apply(@event);
        }
    }
}
