using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.LoadBalancer
{
    public class DefaultLoadBalancerFactory<T> : ILoadBalancerFactory<T>
    {

        public ILoadBalancer<T> Get(Func<List<T>> func, LoadBalancerType Type = LoadBalancerType.Random)
        {
            switch (Type)
            {
                case LoadBalancerType.Random:
                    return new RandomLoadBalancer<T>(func);
                default:
                    throw new ArgumentException("Undefined load balancer");
            }
        }
    }
}
