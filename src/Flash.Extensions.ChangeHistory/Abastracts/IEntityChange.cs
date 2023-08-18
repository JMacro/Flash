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
        /// <param name="oldObj"></param>
        /// <param name="newObj"></param>
        /// <returns></returns>
        ChangeHistoryInfo Compare(Object oldObj, Object newObj);
        /// <summary>
        /// 记录变更
        /// </summary>
        /// <typeparam name="TChangeObject"></typeparam>
        /// <param name="oldObj"></param>
        /// <param name="newObj"></param>
        /// <returns></returns>
        Task<bool> Record<TChangeObject>(TChangeObject oldObj, TChangeObject newObj) where TChangeObject : IEntityChangeTracking;
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
