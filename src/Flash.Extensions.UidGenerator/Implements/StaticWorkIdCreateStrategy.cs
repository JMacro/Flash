namespace Flash.Extensions.UidGenerator.WorkIdCreateStrategy
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“StaticWorkIdCreateStrategy”的 XML 注释
    public class StaticWorkIdCreateStrategy : IWorkIdCreateStrategy
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“StaticWorkIdCreateStrategy”的 XML 注释
    {
        private readonly int _WorkId;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员“StaticWorkIdCreateStrategy.StaticWorkIdCreateStrategy(int)”的 XML 注释
        public StaticWorkIdCreateStrategy(int WorkId)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“StaticWorkIdCreateStrategy.StaticWorkIdCreateStrategy(int)”的 XML 注释
        {
            _WorkId = WorkId;

        }
        
        /// <summary>
        /// 获取1~32之间的数字
        /// </summary>
        /// <returns></returns>
        public int NextId()
        {
            return _WorkId;
        }
    }
}
