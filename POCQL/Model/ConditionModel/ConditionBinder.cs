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
    /// 條件物件&SQL條件鏈結
    /// </summary>
    public class ConditionBinder : List<ConditionBinder.ConditionBinderSet>
    {
        /// <summary>
        /// Add ConditionSet
        /// </summary>
        /// <param name="customCondition">Custom Condition</param>
        public void Add(string customCondition)
        {
            if (!string.IsNullOrEmpty(customCondition))
                base.Add(new ConditionBinderSet(new ConditionSet(customCondition)));
        }

        /// <summary>
        /// Add ConditionSets
        /// </summary>
        /// <param name="customCondition">Custom Condition</param>
        /// <param name="filter">Binding Filter</param>
        public void Add(string customCondition, Func<bool> filter)
        {
            if (!string.IsNullOrEmpty(customCondition))
                base.Add(new ConditionBinderSet(new ConditionSet(customCondition), filter));
        }

        /// <summary>
        /// 解析並取出條件物件中不為null的Property，對應的ConditionSet
        /// </summary>
        /// <param name="conditionObj">Custom Condition</param>
        /// <returns></returns>
        public IEnumerable<ConditionSet> ParseCondition()
        {
            foreach (var member in this)
            {
                if ((member.Filter ?? new Func<bool>(() => true)).Invoke())
                    yield return member.Condition;
            }
        }

        /// <summary>
        /// ConditionBinder Set
        /// </summary>
        public class ConditionBinderSet
        {
            internal ConditionBinderSet(ConditionSet condition)
            {
                this.Condition = condition;
            }

            internal ConditionBinderSet(ConditionSet condition, Func<bool> filter)
                : this(condition)
            {
                this.Filter = filter;
            }

            /// <summary>
            /// Custom ConditionSet
            /// </summary>
            public ConditionSet Condition { get; protected set; }

            /// <summary>
            /// 執行鏈結的條件
            /// </summary>
            public Func<bool> Filter { get; protected set; }
        }
    }

    /// <summary>
    /// 條件物件&SQL條件鏈結
    /// </summary>
    /// <typeparam name="T">傳入條件物件型態</typeparam>
    public class ConditionBinder<T> : List<ConditionBinder<T>.ConditionBinderSet<T>>
        where T : class
    {
        /// <summary>
        /// Add ConditionSet
        /// </summary>
        /// <param name="exp">Lambda Expression</param>
        /// <param name="customCondition">Custom Condition</param>
        public void Add(string customCondition)
        {
            this.Add(customCondition, null, null);
        }

        /// <summary>
        /// Add ConditionSets
        /// </summary>
        /// <param name="customConditions">Custom Conditions</param>
        public void Add(IEnumerable<string> customConditions)
        {
            this.AddRange( customConditions.Where(i=> !string.IsNullOrEmpty(i))
                                           .Select(i=> new ConditionBinderSet<T>(new ConditionSet(i), null, null)));
        }

        /// <summary>
        /// Add ConditionSet
        /// </summary>
        /// <param name="exp">Lambda Expression</param>
        /// <param name="customCondition">Custom Condition</param>
        public void Add(string customCondition, Expression<Func<T, object>> exp)
        {
            this.Add(customCondition, exp, null);
        }

        /// <summary>
        /// Add ConditionSet
        /// </summary>
        /// <param name="exp">Lambda Expression</param>
        /// <param name="customCondition">Custom Condition</param>
        /// <param name="filter">Binding Filter</param>
        public void Add(string customCondition, Expression<Func<T, object>> exp, Func<bool> filter)
        {
            if (!string.IsNullOrEmpty(customCondition))
                base.Add(new ConditionBinderSet<T>(new ConditionSet(customCondition), exp, filter));
        }

        /// <summary>
        /// 解析並取出條件物件中不為null的Property，對應的ConditionSet
        /// </summary>
        /// <param name="conditionObj"></param>
        /// <returns></returns>
        public IEnumerable<ConditionSet> ParseCondition(T conditionObj)
        {
            Type objType = conditionObj.GetType();
            PropertyInfo propInfo;

            foreach (var member in this)
            {
                // *** 如果沒有鏈結物件Property，就直接回傳ConditionSet ***
                if (member.BindingProperty == null)
                {
                    yield return member.Condition;
                    continue;
                }

                // *** 如果有鏈結物件Property，就要依Property Value是否有值來決定
                propInfo = objType.GetProperty(member.BindingProperty.GetPropertyName());
                if ((member.Filter ?? new Func<bool>(() => propInfo.GetValue(conditionObj) != null)).Invoke())
                    yield return member.Condition;
            }
        }

        /// <summary>
        /// ConditionBinder Set
        /// </summary>
        /// <typeparam name="TCondition"></typeparam>
        public class ConditionBinderSet<TCondition> : ConditionBinder.ConditionBinderSet
        {
            internal ConditionBinderSet(ConditionSet condition) : base(condition) { }

            internal ConditionBinderSet(ConditionSet condition, Expression<Func<TCondition, object>> bindingProperty)
                : base(condition)
            {
                this.BindingProperty = bindingProperty;
            }

            internal ConditionBinderSet(ConditionSet condition, Expression<Func<TCondition, object>> bindingProperty, Func<bool> filter)
                : this(condition, bindingProperty)
            {
                this.Filter = filter;
            }

            /// <summary>
            /// 鏈結Property
            /// </summary>
            public Expression<Func<TCondition, object>> BindingProperty { get; private set; }
        }
    }




}
