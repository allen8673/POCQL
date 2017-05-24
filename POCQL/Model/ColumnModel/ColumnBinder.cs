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
    /// Entity & Column鏈結
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    public class ColumnBinder<TDomain> : ColumnBinder<TDomain, TDomain>
    {
        public override IEnumerable<ColumnSet> ParseColumn()
        {
            foreach (var member in this)
            {
                // *** 如果沒有鏈結物件Property，就直接回傳ColumnSet ***
                if (member.BindingProperty == null)
                {
                    yield return new ColumnSet(member.Column);
                    continue;
                }

                yield return new ColumnSet(member.Column, member.BindingProperty.GetPropertyName());
            }
        }
    }

    /// <summary>
    /// Entity & Column鏈結
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TView"></typeparam>
    public class ColumnBinder<TDomain, TView> : List<ColumnBinder<TDomain, TView>.ColumnBinderSet>
    {
        /// <summary>
        /// Add ColumnSet
        /// </summary>
        /// <param name="column"></param>
        /// <param name="exp"></param>
        public void Add(string column)
        {
            this.Add(new ColumnBinderSet(column, null));
        }

        /// <summary>
        /// Add ColumnSet
        /// </summary>
        /// <param name="column"></param>
        /// <param name="exp"></param>
        public void Add(string column, Expression<Func<TDomain, object>> exp)
        {
            base.Add(new ColumnBinderSet(column, exp));
        }

        /// <summary>
        /// 解析並取出條件物件中的Property，對應的ColumnSet
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<ColumnSet> ParseColumn()
        {
            string propName;

            foreach (var member in this)
            {
                // *** 如果沒有鏈結物件Property，就直接回傳ColumnSet ***
                if (member.BindingProperty == null)
                {
                    yield return new ColumnSet(member.Column);
                    continue;
                }

                // *** 取得Domain物件鏈結的Property Name，並比對View Model是否也有相同的Property Name ***
                propName = member.BindingProperty.GetPropertyName();
                if (typeof(TView).GetProperty(propName) != null)
                    yield return new ColumnSet(member.Column, propName);
            }
        }

        /// <summary>
        /// ColumnBinder Set
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        public class ColumnBinderSet
        {
            internal ColumnBinderSet(string column, Expression<Func<TDomain, object>> bindingProperty)
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
            public Expression<Func<TDomain, object>> BindingProperty { get; private set; }
        }
    }
}
