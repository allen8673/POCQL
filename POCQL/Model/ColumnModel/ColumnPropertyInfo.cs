using POCQL.Model.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model
{
    internal class ColumnPropertyInfo
    {
        public ColumnPropertyInfo(PropertyInfo prop, string defaultTable, object value, IEnumerable<string> nullableList)
        {
            this.NullableList = nullableList;
            this.PropInfo = prop;
            this.DefaultTable = defaultTable;
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
                // *** 設定PropertyInfo本身資訊 ***
                _PropInfo = value;
                this.Name = _PropInfo.Name;
                // *** 設定ColumnMapperAttribute資訊 ***
                this.ColumnInfo = _PropInfo.GetCustomAttribute<ColumnMapperAttribute>(true) as ColumnMapperAttribute;
                // *** 檢核是否為Primary Key ***
                PrimaryKeyAttribute primaryKeyAttr = _PropInfo.GetCustomAttribute<PrimaryKeyAttribute>(true) as PrimaryKeyAttribute;
                this.IsPrimaryKey = primaryKeyAttr != null;
                // *** 檢核是否NULLAble ***
                this.Nullable = (_PropInfo.GetCustomAttribute<NullableAttribute>(true) as NullableAttribute) != null
                                || this.NullableList.Contains(this.Name);

                // *** 檢核是否Read Only ***
                ReadOnlyAttribute readOnlyAttr = _PropInfo.GetCustomAttribute<ReadOnlyAttribute>(true) as ReadOnlyAttribute;
                this.ReadOnly = readOnlyAttr != null ||
                                (primaryKeyAttr != null && primaryKeyAttr.AutoIncrement);
                // ***判斷是否是為Aggregate欄位；
                //     如果是Aggregate，就要取Aggregate Function ***
                this.AggregationInfo = _PropInfo.GetCustomAttribute<AggregationAttribute>(true) as AggregationAttribute;

            }
        }
        PropertyInfo _PropInfo;

        /// <summary>
        /// Property Value
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Property Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Property是否為Primary Key
        /// </summary>
        public bool IsPrimaryKey { get; private set; }

        /// <summary>
        /// Property是否Nullable
        /// </summary>
        public bool Nullable { get; private set; }

        /// <summary>
        /// Property是否ReadOnly
        /// </summary>
        public bool ReadOnly { get; private set; }

        /// <summary>
        /// NULLable Property 白名單
        /// </summary>
        IEnumerable<string> NullableList
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


        #region ColumnMapperAttribute Info
        /// <summary>
        /// 設定ColumnMapperAttribute相關資訊
        /// </summary>
        private ColumnMapperAttribute ColumnInfo
        {
            set
            {
                this.HasColumnSetting = value != null;
                this.Column = HasColumnSetting ? value.Column : string.Empty;
                this.Table = HasColumnSetting ? value.Table : string.Empty;
                this.MapType = HasColumnSetting ? value.MapType : 0;
            }
        }

        /// <summary>
        /// 是否有ColumnMapperAttribute的設定
        /// </summary>
        public bool HasColumnSetting { get; private set; }

        /// <summary>
        /// Property對應的Column
        /// </summary>
        private string _Column;
        public string Column
        {
            get
            {
                return string.IsNullOrEmpty(this._Column) ? this.Name : this._Column;
            }
            private set
            {
                this._Column = value;
            }
        }

        /// <summary>
        /// 預設對應Table
        /// </summary>
        private string DefaultTable { get; set; }

        /// <summary>
        /// Property對應的Table
        /// </summary>
        public string Table
        {
            get { return string.IsNullOrEmpty(_Table) ? this.DefaultTable : _Table; }
            private set { _Table = value; }
        }
        private string _Table;

        /// <summary>
        /// 映射型態
        /// </summary>
        public ColumnType MapType { get; private set; }
        #endregion

        #region AggregationAttribute Info
        /// <summary>
        /// 設定Aggregate相關資訊
        /// </summary>
        private AggregationAttribute AggregationInfo
        {
            set
            {
                this.IsAggregate = value != null;
                this.AggregateOperator = this.IsAggregate ? value.AggregateFunction : string.Empty;
            }
        }

        /// <summary>
        /// Property是否為Aggregate運算
        /// </summary>
        public bool IsAggregate { get; private set; }

        /// <summary>
        /// 如果Property為Aggregate運算，則其運算式
        /// </summary>
        public string AggregateOperator { get; private set; }

        #endregion

    }
}
