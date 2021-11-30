using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Flash.Extensions.ORM
{
    public sealed class OrderBy
    {
        private OrderBy(Expression orderField, PageOrderBy orderMode)
        {
            OrderField = orderField;
            OrderMode = orderMode;
        }

        /// <summary>
        /// 排序字段
        /// </summary>
        public Expression OrderField { get; set; }
        /// <summary>
        /// 排序方式
        /// </summary>
        public PageOrderBy OrderMode { get; set; } = PageOrderBy.DESC;

        public static OrderBy Create<TQueryableEntity, TKey>(TQueryableEntity entity, Expression<Func<TQueryableEntity, TKey>> orderField, PageOrderBy orderMode)
        {
            return new OrderBy(orderField, orderMode);
        }
    }

    public sealed class OrderByCollection
    {
        private readonly List<OrderBy> _orderBy = new List<OrderBy>();

        public OrderByCollection Add(OrderBy orderBy)
        {
            _orderBy.Add(orderBy);
            return this;
        }

        public List<OrderBy> ToList()
        {
            return _orderBy;
        }
    }
}
