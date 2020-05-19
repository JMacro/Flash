using System;
using System.Collections.Generic;

namespace Flash.LoadBalancer
{
    /// <summary>
    /// 哈希负载均衡器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HashLoadBalancer<T> : ILoadBalancer<T>
    {
        private readonly Func<List<T>> _func;
        private readonly object _lock = new object();
        private int _index = 0;

        public HashLoadBalancer(Func<List<T>> func)
        {
            this._func = func;
        }

        public T Resolve()
        {
            throw new NotImplementedException();
        }

        public T Resolve(List<T> connections)
        {
            throw new NotImplementedException();
        }
    }
}
