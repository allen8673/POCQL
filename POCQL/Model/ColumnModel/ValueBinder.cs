using POCQL.Model;
using POCQL.Process.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POCQL
{
    /// <summary>
    /// Entity Value & Column鏈結
    /// 僅僅是依"Entity Value有沒有值"決定是否要做鏈結
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ValueBinder<TEntity> : List<ValueBinder<TEntity>.ValueBinderSet>
    {
        public ValueBinder(TEntity entityObject)
        {
            EntityObject = entityObject;
            EntityType = typeof(TEntity);
        }

        /// <summary>
        /// Entity物件
        /// </summary>
        private TEntity EntityObject { get; set; }

        /// <summary>
        /// Entity型別
        /// </summary>
        private Type EntityType { get; set; }

        /// <summary>
        /// Add ColumnSet
        /// </summary>
        /// <param name="column">從指定欄位對應資料來源字串(格式:{Column}:{DataSource})設定Column資訊</param>
        /// <param name="exp"></param>
        public void Add(string column)
        {
            this.Add(new ValueBinderSet(column, null));
        }

        /// <summary>
        /// Add ColumnSet
        /// </summary>
        /// <param name="column"></param>
        /// <param name="exp"></param>
        public void Add(string column, Expression<Func<TEntity, object>> exp)
        {
            base.Add(new ValueBinderSet(column, exp));
        }

        /// <summary>
        /// 解析並取出條件物件中的Property，對應的ColumnSet
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ColumnSet> ParseColumn()
        {
            PropertyInfo propinfo;
            string propName;
            object value;

            foreach (var member in this)
            {
                // *** 如果沒有鏈結物件Property，就直接回傳ColumnSet ***
                if (member.BindingProperty == null)
                {
                    yield return ColumnSet.ParseDataSource(member.Column, false, false, false);
                    continue;
                }

                // *** 取得Entity物件的Value，如果Value有值就回傳鏈結COLUMN ***
                propName = member.BindingProperty.GetPropertyName();
                propinfo = EntityType.GetProperty(propName);
                value = propinfo.GetValue(EntityObject);

                if (value != null)
                    yield return ColumnSet.ParseDataSource(member.Column, false, false, false);
            }
        }

        /// <summary>
        /// ValueBinder Set
        /// </summary>
        public class ValueBinderSet
        {
            internal ValueBinderSet(string column, Expression<Func<TEntity, object>> bindingProperty)
            {
                this.Column = column;
                this.BindingProperty = bindingProperty;
            }

            /// <summary>
            /// Column
            /// </summary>
            public string Column { get; protected set; }

            /// <summary>
            /// 鏈結Property
            /// </summary>
            public Expression<Func<TEntity, object>> BindingProperty { get; private set; }
        }
    }
}
