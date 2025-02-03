using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.LoadBalancer
{
    public enum LoadBalancerType
    {
        /// <summary>
        /// 随机均衡机
        /// </summary>
        Random = 1,
        /// <summary>
        /// 轮询均衡机
        /// </summary>
        RoundRoin = 2,
    }
}
