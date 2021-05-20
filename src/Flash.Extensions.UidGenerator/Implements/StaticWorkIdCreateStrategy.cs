﻿namespace Flash.Extensions.UidGenerator.WorkIdCreateStrategy
{
    public class StaticWorkIdCreateStrategy : IWorkIdCreateStrategy
    {
        private readonly int _WorkId;
        public StaticWorkIdCreateStrategy(int WorkId)
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
