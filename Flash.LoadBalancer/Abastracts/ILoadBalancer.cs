using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.LoadBalancer
{
    public interface ILoadBalancer<T>
    {
        T Get();

        T Get(IEnumerable<T> connections);
    }
}
