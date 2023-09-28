using Flash.AspNetCore.WorkFlow.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Events
{
    internal class FormTypeInitEvent : DomainEventBase
    {
        public string Name { get; set; }
        public IDictionary<string, string> Fields { get; set; }
    }
}
