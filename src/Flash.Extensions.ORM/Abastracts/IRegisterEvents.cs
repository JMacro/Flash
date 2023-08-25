﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.ORM
{
    public interface IRegisterEvents
    {
        /// <summary>
        /// 属性变更事件
        /// <para>可用于全局实体变更日志记录处理</para>
        /// </summary>
        Action<EntityChangeTracker> StateChanged { get; set; }
#if NET6_0
        /// <summary>
        /// 保存变更前事件
        /// </summary>
        Action SavingChanges { get; set; }
        /// <summary>
        /// 保存变更成功事件
        /// </summary>
        Action SavedChanges { get; set; }
        /// <summary>
        /// 保存变更失败事件
        /// </summary>
        Action SaveChangesFailed { get; set; }
#endif
    }
}
