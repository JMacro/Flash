﻿using Flash.Extensions.CompareObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flash.Extensions.ChangeHistory
{
    /// <summary>
    /// 实体变更帮助类
    /// </summary>
    public class EntityChange : IEntityChange
    {
        /// <summary>
        /// 寄存器
        /// </summary>
        private readonly IStorage _storage;
        /// <summary>
        /// 对比器
        /// </summary>
        private readonly ICompareLogic _compareLogic;

        public EntityChange(IStorage storage, ICompareLogic compareLogic)
        {
            this._storage = storage;
            this._compareLogic = compareLogic;
        }

        public ChangeHistoryInfo Compare(Object oldObj, Object newObj)
        {
            if (oldObj == null && newObj == null) return default;

            var typeChangeObject = GetType(oldObj, newObj);
            if (typeChangeObject == null) return default;

            if (!typeof(IEntityChangeTracking).IsAssignableFrom(typeChangeObject))
            {
                throw new ArgumentException($"输入对象未实现{nameof(IEntityChangeTracking)}接口");
            }

            object changeObjectId = null;
            if (oldObj != null) changeObjectId = (oldObj as IEntityChangeTracking).ChangeObjectId;
            else if (newObj != null) changeObjectId = (newObj as IEntityChangeTracking).ChangeObjectId;

            var result = new ChangeHistoryInfo
            {
                EntityId = changeObjectId,
                EntityType = typeChangeObject.FullName,
                HistoryPropertys = new List<ChangeHistoryPropertyInfo>()
            };

            var propertyInfos = typeChangeObject.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.CustomAttributes.Is<IgnoreCheckAttribute>())
                {
                    this._compareLogic.Config.MembersToIgnore.Add($"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");
                }
            }

            var compareResult = this._compareLogic.Compare(oldObj, newObj);
            if (compareResult.AreEqual) return result;

            result.HistoryPropertys.AddRange(compareResult.Differences.Select(p => new ChangeHistoryPropertyInfo
            {
                OldValue = p.Object1,
                NewValue = p.Object2,
                PropertyName = p.PropertyName
            }));
            return result;
        }

        public async Task<bool> Record<TChangeObject>(TChangeObject oldObj, TChangeObject newObj) where TChangeObject : IEntityChangeTracking
        {
            var compareResult = Compare(oldObj, newObj);
            if (compareResult == null || !compareResult.HistoryPropertys.Any()) return false;

            return await _storage.Insert(compareResult);
        }

        public async Task<bool> Record<T>(ChangeHistoryInfo historie)
        {
            if (historie == null || !historie.HistoryPropertys.Any()) return false;

            return await _storage.Insert(historie);
        }

        public async Task<IBasePageResponse<ChangeHistoryInfo>> GetPageList(PageSearchQuery page)
        {
            return await this._storage.GetPageList(page);
        }

        private Type GetType(Object oldObj, Object newObj)
        {
            if (oldObj == null && newObj == null) return null;

            var typeChangeObject = default(Type);
            var typeOldChangeObject = default(Type);
            if (oldObj != null) typeChangeObject = typeOldChangeObject = oldObj.GetType();

            var typeNewChangeObject = default(Type);
            if (newObj != null) typeChangeObject = typeNewChangeObject = newObj.GetType();

            if (oldObj != null && newObj != null && typeOldChangeObject != typeNewChangeObject) throw new ArgumentException($"参数{nameof(oldObj)}与{nameof(newObj)}类型不一致，无法比较");

            if (typeChangeObject == null) return null;

            return typeChangeObject;
        }
    }
}