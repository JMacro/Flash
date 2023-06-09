using System;
using System.Collections.Generic;
using System.Text;
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

        public EntityChange(IStorage storage)
        {
            this._storage = storage;
        }

        public async Task<bool> Record<T>(T oldObj, T newObj, string entityId, string changeUserId, string remark = "") where T : class, new()
        {
            if (oldObj.Equals(newObj)) return true;

            var histories = new List<ChangeHistoryInfo>();

            var objType = typeof(T);
            var propertyInfos = objType.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.CustomAttributes.Is<IgnoreCheckAttribute>()) continue;

                var oldValue = propertyInfo.GetValue(oldObj);
                var newValue = propertyInfo.GetValue(newObj);

                if (oldValue == null && newValue == null) continue;
                if (oldValue != null && oldValue.Equals(newValue)) continue;
                if (newValue != null && newValue.Equals(oldValue)) continue;

                (string OldValue, string NewValue) propertyValue = ("", "");

                if (propertyInfo.PropertyType.IsEnum || propertyInfo.PropertyType.IsNullableEnum())
                {
                    propertyValue = PropertyValueHandler(oldValue, newValue, (value) => value.GetEnumDescript().IsNullOrEmpty() ? ((int)value).ToString() : value.GetEnumDescript());
                }
                else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(Nullable<DateTime>))
                {
                    propertyValue = PropertyValueHandler(oldValue, newValue, (value) => ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    propertyValue = PropertyValueHandler(oldValue, newValue, (value) => Convert.ToString(value));
                }

                histories.Add(new ChangeHistoryInfo
                {
                    EntityType = objType.FullName,
                    EntityId = entityId,
                    PropertyName = propertyInfo.Name,
                    OldValue = propertyValue.OldValue,
                    NewValue = propertyValue.NewValue,
                    ChangeUserId = changeUserId,
                    CreateTime = DateTime.Now,
                    Remark = remark
                });
            }
            return await _storage.Insert(histories.ToArray());

            (string OldValue, string NewValue) PropertyValueHandler(object oldObjValue, object newObjValue, Func<object, string> func)
            {
                var OldValue = "";
                var NewValue = "";

                if (oldObjValue == null)
                {
                    OldValue = "";
                }
                else
                {
                    OldValue = func(oldObjValue);
                }

                if (newObjValue == null)
                {
                    NewValue = "";
                }
                else
                {
                    NewValue = func(newObjValue);
                }
                return (OldValue, NewValue);
            }
        }

        public async Task<IBasePageResponse<ChangeHistoryInfo>> GetPageList(PageSearchQuery page)
        {
            return await this._storage.GetPageList(page);
        }
    }
}