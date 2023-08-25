using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extensions.ChangeHistory
{
    /// <summary>
    /// 实体变更
    /// </summary>
    public interface IEntityChange
    {
        /// <summary>
        /// 检测变更
        /// </summary>
        /// <typeparam name="TEntityIdType"></typeparam>
        /// <param name="entityId"></param>
        /// <param name="oldObj"></param>
        /// <param name="newObj"></param>
        /// <returns></returns>
        ChangeHistoryInfo Compare<TEntityIdType>(TEntityIdType entityId, Object oldObj, Object newObj) where TEntityIdType : struct;
        /// <summary>
        /// 记录变更
        /// </summary>
        /// <typeparam name="TEntityIdType"></typeparam>
        /// <typeparam name="TChangeObject"></typeparam>
        /// <param name="entityId"></param>
        /// <param name="oldObj"></param>
        /// <param name="newObj"></param>
        /// <returns></returns>
        Task<bool> Record<TEntityIdType, TChangeObject>(TEntityIdType entityId, TChangeObject oldObj, TChangeObject newObj) where TEntityIdType : struct where TChangeObject : IEntityChangeTracking;
        /// <summary>
        /// 记录变更
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="historie"></param>
        /// <returns></returns>
        Task<bool> Record<T>(ChangeHistoryInfo historie);
        /// <summary>
        /// 获得分页记录
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        Task<IBasePageResponse<ChangeHistoryInfo>> GetPageList(PageSearchQuery page);
    }
}
