using Flash.Extensions.ORM;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace System.Linq
{
    public enum OperatorType
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal,
        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual,
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 大于等于
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// 小于
        /// </summary>
        LessThan,
        /// <summary>
        /// 小于等于
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// 完全模糊匹配
        /// </summary>
        Like,
        /// <summary>
        /// 左模糊匹配
        /// </summary>
        LeftLike,
        /// <summary>
        /// 右模糊匹配
        /// </summary>
        RightLike,
        /// <summary>
        /// 包含
        /// </summary>
        In
    }

    public static class IQueryableExtensions
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TQueryableEntity"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="page">分页参数</param>
        /// <param name="isCount">是否计算总数</param>
        /// <returns></returns>
        public static async Task<IBasePageResponse<TQueryableEntity>> QueryPageAsync<TQueryableEntity, TKey>(this IQueryable<TQueryableEntity> queryable,
            IPageQuery page,
            Expression<Func<TQueryableEntity, TKey>> orderBy,
            bool isCount = false)
        {
            page = page ?? throw new ArgumentNullException("分页参数不允许为空");
            page.PageIndex = page.PageIndex <= 0 ? 1 : page.PageIndex;
            page.PageSize = page.PageSize <= 0 ? 20 : page.PageSize;

            if (isCount)
            {
                var count = await queryable.CountAsync();

                if (orderBy != null)
                {
                    queryable = (page.OrderBy == PageOrderBy.DESC ? queryable.OrderByDescending(orderBy) : queryable.OrderBy(orderBy));
                }

                var list = default(List<TQueryableEntity>);

                if (count > 0)
                {
                    queryable = queryable.Skip((page.PageIndex - 1) * page.PageSize).Take(page.PageSize);
                    list = await queryable.ToListAsync();
                }               

                return new PageCountResponse<TQueryableEntity>(list, page.PageIndex, page.PageSize, count);
            }
            else
            {
                if (orderBy != null)
                {
                    queryable = (page.OrderBy == PageOrderBy.DESC ? queryable.OrderByDescending(orderBy) : queryable.OrderBy(orderBy));
                }

                queryable = queryable.Skip((page.PageIndex - 1) * page.PageSize).Take(page.PageSize);
                var list = await queryable.ToListAsync();
                return new PageNotCountResponse<TQueryableEntity>(list, page.PageIndex, page.PageSize);
            }
        }

        /// <summary>
        /// 条件查询，如条件值为空则不参与条件过滤
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TConditionSource"></typeparam>
        /// <typeparam name="TEntityProperty"></typeparam>
        /// <typeparam name="TConditionProperty"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="searchData"></param>
        /// <param name="leftProperty"></param>
        /// <param name="rightProperty"></param>
        /// <param name="operatorType"></param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereWith<TSource, TConditionSource, TEntityProperty, TConditionProperty>(
            this IQueryable<TSource> queryable,
            TConditionSource searchData,
            Expression<Func<TSource, TEntityProperty>> leftProperty,
            Func<TConditionSource, TConditionProperty> rightProperty,
            OperatorType operatorType)
        {
            var conditionValue = rightProperty(searchData);
            if (conditionValue == null)
            {
                return queryable;
            }

            if (conditionValue.GetType() == typeof(String))
            {
                if (string.IsNullOrEmpty(conditionValue.ToString()))
                {
                    return queryable;
                }
            }

            if (typeof(IList).IsAssignableFrom(conditionValue.GetType()))
            {
                if ((conditionValue as ICollection).Count <= 0)
                {
                    return queryable;
                }
            }

            BinaryExpression binaryExpression = default;
            switch (operatorType)
            {
                case OperatorType.Equal:
                    binaryExpression = Expression.Equal(leftProperty.Body, Expression.Constant(conditionValue));
                    break;
                case OperatorType.NotEqual:
                    binaryExpression = Expression.NotEqual(leftProperty.Body, Expression.Constant(conditionValue));
                    break;
                case OperatorType.GreaterThan:
                    binaryExpression = Expression.GreaterThan(leftProperty.Body, Expression.Constant(conditionValue));
                    break;
                case OperatorType.GreaterThanOrEqual:
                    binaryExpression = Expression.GreaterThanOrEqual(leftProperty.Body, Expression.Constant(conditionValue));
                    break;
                case OperatorType.LessThan:
                    binaryExpression = Expression.LessThan(leftProperty.Body, Expression.Constant(conditionValue));
                    break;
                case OperatorType.LessThanOrEqual:
                    binaryExpression = Expression.LessThanOrEqual(leftProperty.Body, Expression.Constant(conditionValue));
                    break;
                case OperatorType.Like:

                    break;
                case OperatorType.LeftLike:

                    break;
                case OperatorType.RightLike:

                    break;
                case OperatorType.In:
                    var collection = (conditionValue as IList);
                    if (collection.Count == 1)
                    {
                        binaryExpression = Expression.Equal(leftProperty.Body, Expression.Constant(collection[0]));
                    }
                    else
                    {
                        Expression body = Expression.Call(Expression.Constant(conditionValue), typeof(TConditionProperty).GetMethod("Contains", new Type[] { typeof(TEntityProperty) }), leftProperty.Body);
                        queryable = queryable.Where(Expression.Lambda<Func<TSource, bool>>(body, leftProperty.Parameters));
                        return queryable;
                    }
                    break;
                default:
                    break;
            }

            queryable = queryable.Where(Expression.Lambda<Func<TSource, bool>>(binaryExpression, leftProperty.Parameters));
            return queryable;
        }
    }
}
