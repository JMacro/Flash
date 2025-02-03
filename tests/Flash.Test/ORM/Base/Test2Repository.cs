using Flash.Extensions.ORM;
using Flash.Extensions.ORM.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Test.ORM.Base
{
    public class Test2Repository : Repository<AccountInfo>, ITest2Repository
    {
        public Test2Repository(TestDb2Context context) : base(context)
        {
        }
    }

    public interface ITest2Repository : IRepository<AccountInfo> { }
}
