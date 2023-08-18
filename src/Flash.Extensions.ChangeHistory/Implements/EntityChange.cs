using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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

        public ChangeHistoryInfo Compare(Object oldObj, Object newObj)
        {
            if (oldObj == null && newObj == null) return default;

            var typeChangeObject = GetType(oldObj, newObj);
            if (typeChangeObject == null) return default;

            object changeObjectId = null;
            if (oldObj != null) changeObjectId = (oldObj as IEntityChangeTracking).ChangeObjectId;
            else if (newObj != null) changeObjectId = (newObj as IEntityChangeTracking).ChangeObjectId;

            var result = new ChangeHistoryInfo
            {
                EntityId = changeObjectId,
                EntityType = typeChangeObject.FullName,
                HistoryPropertys = new List<ChangeHistoryPropertyInfo>()
            };

            if (oldObj.Equals(newObj)) return result;

            InternalCompare(oldObj, newObj, result.HistoryPropertys);


            var propertyInfos = typeChangeObject.GetProperties();
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

                result.HistoryPropertys.Add(new ChangeHistoryPropertyInfo
                {
                    PropertyName = propertyInfo.Name,
                    OldValue = propertyValue.OldValue,
                    NewValue = propertyValue.NewValue
                });
            }
            return result;
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

        private FieldInfo InternalCompare(Object oldObj, Object newObj, IList<ChangeHistoryPropertyInfo> changeHistories, FieldInfo originalFieldInfo = null)
        {
            var typeChangeObject = GetType(oldObj, newObj);
            if (typeChangeObject == null) return originalFieldInfo;

            if (typeChangeObject.IsArray)
            {
                var arrayType = typeChangeObject.GetElementType();
                if (!arrayType.IsValueTypeAndPrimitive())
                {
                    Array compareOldArray = (Array)oldObj;
                    Array compareNewArray = (Array)newObj;
                }
            }

            CompareFields(oldObj, newObj, typeChangeObject, changeHistories);
            return originalFieldInfo;
        }



        private void CompareFields(Object oldObj, Object newObj, Type typeToReflect, IList<ChangeHistoryPropertyInfo> changeHistories, FieldInfo originalFieldInfo = null, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (var fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                dynamic oldValue = oldObj != null ? fieldInfo.GetValue(oldObj) : null;
                dynamic newValue = newObj != null ? fieldInfo.GetValue(newObj) : null;
                if (!fieldInfo.FieldType.IsValueTypeAndPrimitive())
                {
                    InternalCompare(oldValue, newValue, changeHistories, fieldInfo);
                }
                else
                {
                    if (oldValue != newValue)
                    {
                        changeHistories.Add(new ChangeHistoryPropertyInfo
                        {
                            OldValue = oldValue != null ? Convert.ToString(oldValue) : "",
                            NewValue = newValue != null ? Convert.ToString(newValue) : "",
                            PropertyName = GetFieldName(fieldInfo)
                        });
                    }
                }

                //if (IsValueTypeAndPrimitive(fieldInfo.FieldType)) continue;
                //var originalFieldValue = fieldInfo.GetValue(originalObject);
                //var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                //fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
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

        private static Regex regex = new Regex("^<[A-Za-z0-9]+>");
        private string GetFieldName(FieldInfo fieldInfo)
        {
            var match = regex.Match(fieldInfo.Name);
            if (match.Success)
            {
                return match.Value.Replace("<", "").Replace(">", "");
            }
            return fieldInfo.Name;
        }
    }
}