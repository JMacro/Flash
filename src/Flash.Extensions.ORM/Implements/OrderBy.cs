using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Flash.Extensions.ORM
{
    /// <summary>
    /// 字段排序
    /// </summary>
    public sealed class OrderBy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderField">字段排序表达式</param>
        /// <param name="orderMode">排序方式</param>
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

        /// <summary>
        /// 创建待排序字段及顺序
        /// </summary>
        /// <typeparam name="TQueryableEntity">实体</typeparam>
        /// <typeparam name="TKey">排序字段</typeparam>
        /// <param name="entity"></param>
        /// <param name="orderField">字段排序表达式</param>
        /// <param name="orderMode">排序方式</param>
        /// <returns></returns>
        public static OrderBy Create<TQueryableEntity, TKey>(TQueryableEntity entity, Expression<Func<TQueryableEntity, TKey>> orderField, PageOrderBy orderMode)
        {
            return new OrderBy(orderField, orderMode);
        }
    }

    /// <summary>
    /// 字段排序收集器
    /// </summary>
    public sealed class OrderByCollection
    {
        private readonly List<OrderBy> _orderBy = new List<OrderBy>();

        /// <summary>
        /// 新增排序，请使用OrderBy.Create()进行添加排序字段
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public OrderByCollection Add(OrderBy orderBy)
        {
            _orderBy.Add(orderBy);
            return this;
        }

        /// <summary>
        /// 新增排序
        /// </summary>
        /// <typeparam name="TQueryableEntity">实体</typeparam>
        /// <typeparam name="TKey">排序字段</typeparam>
        /// <param name="entity"></param>
        /// <param name="orderField">字段排序表达式</param>
        /// <param name="orderMode">排序方式</param>
        /// <returns></returns>
        public OrderByCollection Add<TQueryableEntity, TKey>(TQueryableEntity entity, Expression<Func<TQueryableEntity, TKey>> orderField, PageOrderBy orderMode)
        {
            this.Add(OrderBy.Create(entity, orderField, orderMode));
            return this;
        }

        /// <summary>
        /// 获得排序收集器
        /// </summary>
        /// <returns></returns>
        public List<OrderBy> ToList()
        {
            return _orderBy;
        }
    }
}
