using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extensions.ChangeHistory
{
    /// <summary>
    /// 寄存器接口
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// 写入
        /// </summary>
        /// <returns></returns>
        Task<bool> Insert(params ChangeHistoryInfo[] changes);
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        Task<IBasePageResponse<ChangeHistoryInfo>> GetPageList(PageSearchQuery page);
    }
}
