using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.UidGenerator.WorkIdCreateStrategy
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IWorkIdCreateStrategy”的 XML 注释
    public interface IWorkIdCreateStrategy
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IWorkIdCreateStrategy”的 XML 注释
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IWorkIdCreateStrategy.NextId()”的 XML 注释
        int NextId();
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IWorkIdCreateStrategy.NextId()”的 XML 注释
    }
}
