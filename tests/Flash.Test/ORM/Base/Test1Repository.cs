using Flash.Extensions.ORM;
using Flash.Extensions.ORM.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Test.ORM.Base
{
    public class Test1Repository : Repository<AccountInfo>, ITest1Repository
    {
        public Test1Repository(TestDb1Context context) : base(context)
        {
        }
    }

    public interface ITest1Repository : IRepository<AccountInfo> { }
}
