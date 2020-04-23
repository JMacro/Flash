using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.LoadBalancer
{
    public interface ILoadBalancerFactory<T>
    {
        ILoadBalancer<T> Get(Func<List<T>> func, LoadBalancerType Type);
    }
}
