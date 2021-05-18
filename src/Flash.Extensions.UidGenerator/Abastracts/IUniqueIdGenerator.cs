using System;

namespace Flash.Extensions.UidGenerator
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IUniqueIdGenerator”的 XML 注释
    public interface IUniqueIdGenerator
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IUniqueIdGenerator”的 XML 注释
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IUniqueIdGenerator.NewId()”的 XML 注释
        long NewId();
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IUniqueIdGenerator.NewId()”的 XML 注释

    }
}
