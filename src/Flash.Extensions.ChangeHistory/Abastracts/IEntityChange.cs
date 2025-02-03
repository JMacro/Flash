using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extensions.ChangeHistory
{
    /// <summary>
    /// 实体变更
    /// <para>如未实现<see cref="IStorage"/>接口，则默认使用<see cref="DefaultStorage"/>寄存器组件</para>
    /// </summary>
    public interface IEntityChange
    {
        /// <summary>
        /// 变更历史信息
        /// </summary>
        ChangeHistoryInfo ChangeHistoryInfo { get; }

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
        /// <param name="entityId"></param>
        /// <param name="oldObj"></param>
        /// <param name="newObj"></param>
        /// <returns></returns>
        Task<bool> Record<TEntityIdType>(TEntityIdType entityId, Object oldObj, Object newObj) where TEntityIdType : struct;
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
