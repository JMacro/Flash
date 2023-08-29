using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.Cache
{
    /// <summary>
    /// 分布式锁续期服务接口
    /// </summary>
    public interface IDistributedLockRenewalService
    {
        /// <summary>
        /// 开始续期
        /// </summary>
        /// <param name="check"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        void RunRenewal(DistributedLockRenewalCheck check, CancellationToken cancellationToken = default(CancellationToken));
    }
}
