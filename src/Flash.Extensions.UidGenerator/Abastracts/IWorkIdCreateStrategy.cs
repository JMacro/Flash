using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.UidGenerator.WorkIdCreateStrategy
{
    public interface IWorkIdCreateStrategy
    {
        int NextId();
    }
}
