using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.LoadBalancer
{
    public interface ILoadBalancer<T>
    {
        T Resolve();

        T Resolve(IEnumerable<T> connections);
    }
}
