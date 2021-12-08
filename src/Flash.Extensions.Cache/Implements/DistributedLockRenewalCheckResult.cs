namespace Flash.Extensions.Cache
{
    public sealed class DistributedLockRenewalCheckResult
    {
        public DistributedLockRenewalCheck Data { get; set; }
        public bool CheckResult { get; set; }

        private DistributedLockRenewalCheckResult(DistributedLockRenewalCheck data, bool checkResult)
        {
            this.Data = data;
            this.CheckResult = checkResult;
        }

        public static DistributedLockRenewalCheckResult Create(DistributedLockRenewalCheck data, bool checkResult)
        {
            return new DistributedLockRenewalCheckResult(data, checkResult);
        }
    }
}
