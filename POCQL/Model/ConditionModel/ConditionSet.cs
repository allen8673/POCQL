using POCQL.Extension;
using POCQL.Model.Interface;
using POCQL.Model.InternalAttribute;
using POCQL.Model.MapAttribute;
using POCQL.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace POCQL.Model
{
    /// <summary>
    /// 條件資訊
    /// </summary>
    public class ConditionSet : STMOperator<ConditionSet>, ITableDetail
    {
        internal ConditionSet(string customCondition, AndOrOperator andOr = AndOrOperator.AND) 
        {
            this.CustomCondition = customCondition;
            this.AndOr = andOr;
        }

        internal ConditionSet(string column, string parameterName, ConditionOperator conditionOpt, object value, AndOrOperator andOr = AndOrOperator.AND) 
        {
            this.ParameterName = parameterName;
            this.Column = column;
            this.Operator = conditionOpt;
            this.Value = value;
            this.TableName = string.Empty;
            this.AndOr = andOr;
        }

        internal ConditionSet(string column, string table, string parameterName, ConditionOperator conditionOpt, object value, string subQuery, AndOrOperator andOr = AndOrOperator.AND)
            : this(column, parameterName, conditionOpt, value, andOr)
        {
            if (Regex.IsMatch(table, @"{#\w+#}"))
                this.TableParameter = table.Replace("{#", string.Empty).Replace("#}", string.Empty);
            else
                this.TableName = table;

            this.SubQuery = subQuery;
        }

        #region Property
        /// <summary>
        /// 條件鎖定的Table名
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Table參數，外界設定格式為{#Table參數#}
        /// </summary>
        public string TableParameter { get; private set; }

        /// <summary>
        /// 欄位名
        /// </summary>
        public string Column { get; private set; }

        /// <summary>
        /// 欄位全名 = {Table Name}.{Column Name}
        /// </summary>
        public string FullColumnName { get { return $"{this.TableName}.{this.Column??this.CustomCondition}"; } }

        /// <summary>
        /// 條件運算子
        /// </summary>
        public ConditionOperator Operator { get; private set; }

        /// <summary>
        /// 條件對應的參數名
        /// </summary>
        public string ParameterName { get; private set; }

        /// <summary>
        /// 條件值
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// AND OR 運算子
        /// </summary>
        public AndOrOperator AndOr { get; private set; }

        /// <summary>
        /// 其他指定條件式
        /// </summary>
        public string CustomCondition { get; private set; }

        /// <summary>
        /// 子查詢
        /// </summary>
        private string _SubQuery;
        public string SubQuery {
            get { return this._SubQuery; }
            private set { this._SubQuery = value.IsNullOrEmpty() ? value : $"({value})"; }
        }
        #endregion

        /// <summary>
        /// 依Table參數重新設置TableName
        /// </summary>
        /// <param name="tabelParamMaps">Table參數表</param>
        internal void ResetTable(Dictionary<string, string> tabelParamMaps)
        {
            if (this.TableParameter.IsNullOrEmpty()) return;

            // *** 如果再Table參數的Dic裡有該Key，則要進行轉換 ***
            this.TableName = tabelParamMaps.ContainsKey(this.TableParameter) ?
                             tabelParamMaps[TableParameter] :
                             this.TableParameter;

            this.TableParameter = string.Empty;
        }

        /// <summary>
        /// 解析並取得ConditionMapperAttribute設定的條件式
        /// </summary>
        /// <param name="table">Condition欄位所在Table</param>
        /// <returns>
        /// {Column} {Operator} @{Column}
        ///      EX: CNO_CODE = @CNO_CODE
        /// </returns>
        internal string ConditionParse(string table)
        {
            // *** 如果CustomCondition有值，就直接處理CustomCondition ***
            // !!! 注意: 從建構子的設計上可以知道CustomCondition跟其他Property是不會同時存的 ***
            if (!this.CustomCondition.IsNullOrEmpty()) return this.CustomCondition;

            // *** 由SubQuery是否為空來決定要走'{Column} {Operator} @{Column}'或是'{Column} {Operator} {Column}' ***
            // *** 如果SubQuery為空就走{Column} {Operator} @{Column} ***
            // *** 如果SubQuery不為空就走{Column} {Operator} {Column} ***
            bool defaultDesc = this.SubQuery.IsNullOrEmpty();

            string column = table.IsNullOrEmpty() ? this.Column : $"{table}.{this.Column}";

            // *** 如果Value為NULL，就直接回傳IS/IS NOT NULL運算式
            if (this.Value == null)
                return this.Operator.GetNullDescription().StringFormat(column);

            // *** 如果是Between，邏輯要另外處理 ***
            if (this.Operator == ConditionOperator.Between)
                return this.BetweenProcess(column);

            string desc = defaultDesc ? this.Operator.GetDescription(): 
                                        this.Operator.GetRawDescription();

            return desc.StringFormat(column, defaultDesc ? this.ParameterName : this.SubQuery) ;
        }

        /// <summary>
        /// 針對Between運算式處理
        /// </summary>
        /// <param name="column">Between運算式的目標欄位</param>
        /// <returns></returns>
        private string BetweenProcess(string column) 
        {
            BetweenSetAttribute attr;
            object propValue = null;
            string preParamName = string.Empty, postParamName = string.Empty;

            foreach (var prop in this.Value.GetType().GetProperties())
            {
                attr = prop.GetCustomAttributes(typeof(BetweenSetAttribute), true).FirstOrDefault()
                   as BetweenSetAttribute;

                if (attr == null) continue;
                propValue = prop.GetValue(this.Value);

                if (propValue == null) continue;

                // *** 從新取對應參數的名字: Pre_{Property} or Post_{Property} ***
                if (attr.Set == BetweenSet.PrefixValue)
                {
                    preParamName = attr.Set.GetDescription().StringFormat(this.ParameterName);
                }
                else
                {
                    postParamName = attr.Set.GetDescription().StringFormat(this.ParameterName);
                }
            }

            // *** 防呆 ***
            if (preParamName == string.Empty && postParamName == string.Empty)
                return string.Empty;

            // *** 這邊要處理如果PrefixValue或PostfixValue其中一者為NULL的情況 ***
            // *** 如果PrefixValue = NULL，則要回傳 {Column Value} <= PostfixValue ***
            if (preParamName == string.Empty && postParamName != string.Empty)
                return ConditionOperator.LessOrEqual.GetDescription().StringFormat(column, postParamName);

            // *** 如果PostfixValue = NULL，則要回傳 {Column Value} >= PrefixValue ***
            else if (preParamName != string.Empty && postParamName == string.Empty)
                return ConditionOperator.MoreOrEqual.GetDescription().StringFormat(column, preParamName);

            // *** 其他情況就正常回傳Between運算式 ***
            else
                return this.Operator.GetDescription().StringFormat(column, preParamName, postParamName);
        }
    }
}
