namespace Flash.Extensions.UidGenerator.WorkIdCreateStrategy
{
    public class StaticWorkIdCreateStrategy : IWorkIdCreateStrategy
    {
        private readonly int _WorkId;
        private readonly int _CenterId;

        public StaticWorkIdCreateStrategy(int WorkId, int CenterId = 0)
        {
            _WorkId = WorkId;
            _CenterId = CenterId;
        }

        /// <summary>
        /// 获取1~32之间的数字
        /// </summary>
        /// <returns></returns>
        public int GetWorkId()
        {
            return _WorkId;
        }

        public int GetCenterId() => _CenterId;
    }
}
