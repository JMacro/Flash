using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extersions.UidGenerator.WorkIdCreateStrategy
{
    public interface IWorkIdCreateStrategy
    {
        int NextId();
    }
}
