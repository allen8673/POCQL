using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using POCQL.MSSQL;


namespace POCQL.Model.MapAttribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConditionAttribute : Attribute
    {
        public ConditionAttribute(string column)
        {
            this.Column = column;
            this.Table = string.Empty;
        }

        public ConditionAttribute(string column, string table)
        {
            this.Column = column;
            this.Table = table;
        }


        public ConditionAttribute(string column, ConditionOperator conditionOperator)
            : this(column)
        {
            this.Operator = conditionOperator;
        }

        public ConditionAttribute(string column, string table, ConditionOperator conditionOperator)
            : this(column, table)
        {
            this.Operator = conditionOperator;
        }

        public ConditionAttribute(string column, bool requiredValue)
            : this(column)
        {
            this.RequiredValue = requiredValue;
        }

        public ConditionAttribute(string column, string table, bool requiredValue)
            : this(column, table)
        {
            this.RequiredValue = requiredValue;
        }

        public ConditionAttribute(string column, bool requiredValue, ConditionOperator conditionOperator)
            : this(column, requiredValue)
        {
            this.Operator = conditionOperator;
        }

        public ConditionAttribute(string column, string table, bool requiredValue, ConditionOperator conditionOperator)
            : this(column, table, requiredValue)
        {
            this.Operator = conditionOperator;
        }

        public ConditionAttribute(string column, ConditionOperator conditionOperator, string subQuery)
            : this(column, conditionOperator)
        {
            this.SubQuery = subQuery;
        }

        public ConditionAttribute(string column, string table, ConditionOperator conditionOperator, string subQuery)
            : this(column, table, conditionOperator)
        {
            this.SubQuery = subQuery;
        }

        /// <summary>
        /// 欄位名
        /// </summary>
        public string Column { get; private set; }

        /// <summary>
        /// 欄位所處的Table名
        /// </summary>
        public string Table { get; private set; }

        /// <summary>
        /// 條件運算子
        /// </summary>
        public ConditionOperator Operator { get; private set; }

        /// <summary>
        /// 不允許NULL
        /// </summary>
        public bool RequiredValue { get; private set; }

        /// <summary>
        /// 條件式中的子查詢
        /// </summary>
        public string SubQuery { get; private set; }
    }
}
