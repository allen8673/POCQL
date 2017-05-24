using POCQL.Model.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Process.Helper
{
    /// <summary>
    /// Condition物件Property和Column的連結設定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConditionMapperSetting<T> : ConditionMapperSetting
    {
        public void Add(Expression<Func<T, object>> func, string column)
        {
            base.Add(new ConditionMapperItem
            {
                Property = func.GetPropertyName(),
                Column = column,
                Operator = ConditionOperator.Equal
            });
        }

        public void Add(Expression<Func<T, object>> func, ConditionOperator conditionOperator, string column)
        {
            base.Add(new ConditionMapperItem
            {
                Property = func.GetPropertyName(),
                Column = column,
                Operator = conditionOperator
            });
        }

        public void Add(Expression<Func<T, object>> func, ConditionOperator conditionOperator, string column, string table)
        {
            base.Add(new ConditionMapperItem
            {
                Property = func.GetPropertyName(),
                Column = column,
                Operator = conditionOperator,
                Table = table
            });
        }
    }

    /// <summary>
    /// Condition物件Property和Column的連結設定
    /// </summary>
    public class ConditionMapperSetting : List<ConditionMapperItem>
    {
        public ConditionMapperItem this[string property]
        {
            get
            {
                return this.Where(i => i.Property == property).FirstOrDefault();
            }
        }
    }

    /// <summary>
    /// Condition物件Property和Column的連結設定細項
    /// </summary>
    public class ConditionMapperItem
    {
        internal string Property { get; set; }
        internal string Column { set; get; }
        internal string Table { set; get; }
        internal ConditionOperator Operator { get; set; }


    }
}
