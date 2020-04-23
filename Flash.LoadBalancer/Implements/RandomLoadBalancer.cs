using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flash.LoadBalancer
{
    internal class RandomLoadBalancer<T> : ILoadBalancer<T>
    {
        private readonly Func<List<T>> _func;
        public RandomLoadBalancer(Func<List<T>> func)
        {
            this._func = func;
        }

        private readonly object _lock = new object();

        public T Get()
        {
            var connections = _func();
            return Get(connections);
        }

        public T Get(IEnumerable<T> connections)
        {
            int _last = new Random(Guid.NewGuid().GetHashCode()).Next(connections.Count() - 1);
            lock (_lock)
            {
                if (_last < connections.Count())
                {
                    _last = 0;
                }

                if (_last > connections.Count())
                {
                    _last = 0;
                }

                var next = connections.ToList()[_last];

                return next;
            }
        }
    }
}
