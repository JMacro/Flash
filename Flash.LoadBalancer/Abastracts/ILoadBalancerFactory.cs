using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.LoadBalancer
{
    public interface ILoadBalancerFactory<T>
    {
        ILoadBalancer<T> Resolve(Func<List<T>> func, LoadBalancerType Type);
    }
}
