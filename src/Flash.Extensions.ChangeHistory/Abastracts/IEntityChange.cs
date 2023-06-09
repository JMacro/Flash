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
        /// 记录变更
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldObj"></param>
        /// <param name="newObj"></param>
        /// <param name="entityId">实体对象Id</param>
        /// <param name="changeUserId">变更人Id</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        Task<bool> Record<T>(T oldObj, T newObj, string entityId, string changeUserId, string remark = "") where T : class, new();
        /// <summary>
        /// 获得分页记录
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        Task<IBasePageResponse<ChangeHistoryInfo>> GetPageList(PageSearchQuery page);
    }
}
