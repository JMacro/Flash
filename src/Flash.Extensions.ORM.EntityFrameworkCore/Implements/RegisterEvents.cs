using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.ORM.EntityFrameworkCore
{
    public class RegisterEvents : IRegisterEvents
    {
        public Action<EntityChangeTracker> StateChanged { get; set; }
#if NET6_0
        public Action SavingChanges { get; set; }
        public Action SavedChanges { get; set; }
        public Action SaveChangesFailed { get; set; }
#endif
    }
}
