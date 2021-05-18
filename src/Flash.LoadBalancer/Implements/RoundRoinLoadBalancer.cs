using System;
using System.Collections.Generic;
using System.Linq;

namespace Flash.LoadBalancer
{
    /// <summary>
    /// 轮询负载均衡器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RoundRoinLoadBalancer<T> : ILoadBalancer<T>
    {
        private readonly Func<List<T>> _func;
        private readonly object _lock = new object();
        private int _index = 0;
        public RoundRoinLoadBalancer(Func<List<T>> func)
        {
            this._func = func;
        }

        public T Resolve()
        {
            var connections = _func();
            return Resolve(connections);
        }

        public T Resolve(List<T> connections)
        {
            lock (_lock)
            {
                if (_index >= connections.Count())
                {
                    _index = 0;
                }
                var next = connections.ToList()[_index];
                _index++;
                return next;
            }
        }
    }
}
