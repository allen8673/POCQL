using POCQL.Model.MapAttribute;
using POCQL.Process.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model
{
    /// <summary>
    /// 條件物件所需要的PropertyInfo資訊
    /// </summary>
    internal class ConditionPropertyInfo
    {
        public ConditionPropertyInfo(PropertyInfo prop, object value, IEnumerable<string> nullableList, ConditionMapperSetting conditionSetting)
        {
            this.ConditionMapper = conditionSetting;
            this.NullableList = nullableList;
            this.PropInfo = prop;
            this.Value = value;
        }


        /// <summary>
        /// 條件物件PropertyInfo
        /// </summary>
        public PropertyInfo PropInfo
        {
            get
            {
                return _PropInfo;
            }
            private set
            {
                _PropInfo = value;
                this.Name = _PropInfo.Name;
                // *** 取得 ConditionMapperAttribute ***
                ConditionMapperItem mapItem = this.ConditionMapper[this.Name];
                this.ConditionSetting = mapItem != null ? new ConditionAttribute(mapItem.Column, mapItem.Table, mapItem.Operator)
                                                        : _PropInfo.GetCustomAttribute<ConditionAttribute>() as ConditionAttribute;
                // *** 檢核是否NULLAble ***
                this.Nullable = (_PropInfo.GetCustomAttribute<NullableAttribute>(true) as NullableAttribute) != null
                                || this.NullableList.Contains(this.Name);
            }
        }
        PropertyInfo _PropInfo;

        /// <summary>
        /// 是否要求一定要有值
        /// </summary>
        private bool _RequiredValue
        {
            get
            {
                return this.ConditionSetting != null ? this.ConditionSetting.RequiredValue : false;
            }
        }

        /// <summary>
        /// Property所設定的ConditionAttribute
        /// </summary>
        public ConditionAttribute ConditionSetting { get; private set; }

        /// <summary>
        /// Property是否Nullable
        /// </summary>
        public bool Nullable { get; private set; }

        /// <summary>
        /// 該Property是否為Condition Property
        /// </summary>
        public bool IsConditionProperty { get { return this.ConditionSetting != null; } }

        /// <summary>
        /// 是否略過此Property不處理
        /// </summary>
        public bool PassToProcess { get { return (!IsConditionProperty) || (Value == null && !Nullable); } }

        /// <summary>
        /// Property Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Property Value
        /// </summary>
        public object Value
        {
            get
            {
                if (_Value == null && this._RequiredValue)
                    throw new Exception($"'{this.Name}'設定為不允許NULL，請確認是否有賦予值");

                return this._Value;
            }
            private set
            {
                this._Value = value;
            }
        }
        private object _Value;

        /// <summary>
        /// NULLable Property 白名單
        /// </summary>
        private IEnumerable<string> NullableList
        {
            get
            {
                return _NullableList ?? new string[] { };
            }
            set
            {
                _NullableList = value;
            }
        }
        private IEnumerable<string> _NullableList;

        private ConditionMapperSetting ConditionMapper { get; set; }
    }

    /// <summary>
    /// ConditionPropertyInfo擴充式
    /// </summary>
    static class ConditionPropertyInfo_Ext
    {
        public static IEnumerable<ConditionPropertyInfo> ToConditionProperty<T>(this IEnumerable<PropertyInfo> propInfos, T obj, IEnumerable<string> nullableList, ConditionMapperSetting conditionSetting)
        {
            return propInfos.Select(i => new ConditionPropertyInfo(i, i.GetValue(obj), nullableList, conditionSetting));
        }
    }
}
