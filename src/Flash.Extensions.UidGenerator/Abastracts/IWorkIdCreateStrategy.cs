using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.UidGenerator.WorkIdCreateStrategy
{
    public interface IWorkIdCreateStrategy
    {
        /// <summary>
        /// 获得工作Id
        /// </summary>
        /// <returns></returns>
        int GetWorkId();
        /// <summary>
        /// 获得数据中心Id
        /// </summary>
        /// <returns></returns>
        int GetCenterId();
    }
}
