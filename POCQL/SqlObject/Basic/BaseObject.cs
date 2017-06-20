using POCQL.Model;
using POCQL.Model.MapAttribute;
using POCQL.Process.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.SqlObject
{
    public abstract class BaseObject<TDerive> : STMOperator<TDerive> 
        where TDerive : class
    {
        internal BaseObject()
        {
            this.TabelParameterMaps = new Dictionary<string, string>();
            this.ColumnInfos = new ColumnSet[] { };
            this.ConditionInfo = new ConditionSet[] { };
            this.CteContain = string.Empty;
        }

        #region Property
        /// <summary>
        /// CTE內容
        /// </summary>
        protected string CteContain { get; set; }

        /// <summary>
        /// 資料來源的Table資訊
        /// </summary>
        protected TableSet Table { get; set; }

        /// <summary>
        /// Table參數({#Parameter#})對應的Table
        /// </summary>
        protected Dictionary<string, string> TabelParameterMaps { get; set; }

        /// <summary>
        /// SQL條件式(不包含'WHERE'字串)
        /// </summary>
        protected abstract string Condition { get; set; }

        /// <summary>
        /// 條件資訊
        /// </summary>
        private ConditionSet[] _ConditionInfo;
        internal ConditionSet[] ConditionInfo
        {
            get
            {
                _ConditionInfo.ToList().ForEach(i => i.ResetTable(this.TabelParameterMaps));
                return this._ConditionInfo;
            }
            set { this._ConditionInfo = value != null ? value.Where(i => i != null).ToArray() : value; }
        }

        /// <summary>
        /// 資料欄位資訊
        /// </summary>
        private ColumnSet[] _ColumnInfos;
        internal ColumnSet[] ColumnInfos
        {
            get
            {
                _ColumnInfos.ToList().ForEach(i => i.ResetTable(this.TabelParameterMaps));
                return this._ColumnInfos;
            }

            set
            {
                // !!! 這邊是為了要實現同時從兩個Domain Model解析SQL的功能，
                //     很有可能還是有Bug !!!
                this._ColumnInfos = value.GroupBy(i => $"{i.Property}-{i.ColumnName}")
                                         .Select(g => g.First())
                                         .ToArray();
            }
        }

        /// <summary>
        /// 目前客製專案
        /// </summary>
        protected string Customize { get; set; }
        #endregion

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <returns></returns>
        public abstract SqlSet Output();

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <param name="otherParam">其他指定參數</param>
        /// <returns></returns>
        public abstract SqlSet Output(object otherParam);

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <param name="otherParams">其他指定參數</param>
        /// <returns></returns>
        public abstract IEnumerable<SqlSet> Output(IEnumerable<object> otherParams);

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <typeparam name="T">指定輸出型別</typeparam>
        /// <returns></returns>
        public abstract T Output<T>();

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="otherParam">其他指定參數</param>
        /// <returns></returns>
        public abstract T Output<T>(object otherParam);

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="otherParams">其他指定參數</param>
        /// <returns></returns>
        public abstract IEnumerable<T> Output<T>(IEnumerable<object> otherParams);

        /// <summary>
        /// 設定WHERE資訊
        /// </summary>
        /// <param name="condition">WHERE條件式</param>
        /// <returns></returns>
        public TDerive Where(string condition)
        {
            this.Condition = condition;
            return this as TDerive;
        }

        /// <summary>
        /// 設定WHERE資訊
        /// </summary>
        /// <param name="conditionObj">經由ColumnMapperAttribute設定的條件式物件</param>
        /// <returns></returns>
        public TDerive Where<T>(T conditionObj)
            where T : class
        {
            this.ConditionInfo = ConditionSetHelper.GetCondition(conditionObj)
                                                   .ToArray();
            return this as TDerive;
        }

        /// <summary>
        /// 設定WHERE資訊
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionObj">經由ColumnMapperAttribute設定的條件式物件</param>
        /// <param name="paramTemplate">條件物件參數Template，請以XXX{#PROP#}XX給</param>
        /// <returns></returns>
        public TDerive Where<T>(T conditionObj, string paramTemplate)
            where T : class
        {
            this.ConditionInfo = ConditionSetHelper.GetCondition(conditionObj, paramTemplate)
                                                   .ToArray();
            return this as TDerive;
        }
        
        /// <summary>
        /// 設定WHERE資訊
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionObj">經由ColumnMapperAttribute設定的條件式物件</param>
        /// <param name="binder">條件物件與SQL條件binder</param>
        /// <returns></returns>
        public TDerive Where<T>(T conditionObj, ConditionBinder binder)
            where T : class
        {
            this.ConditionInfo = ConditionSetHelper.GetCondition(conditionObj)
                                                   .Union(binder.ParseCondition())
                                                   .ToArray();
            return this as TDerive;
        }

        /// <summary>
        /// 設定WHERE資訊
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionObj">經由ColumnMapperAttribute設定的條件式物件</param>
        /// <param name="binder">條件物件與SQL條件binder</param>
        /// <returns></returns>
        public TDerive Where<T>(T conditionObj, ConditionBinder<T> binder)
            where T : class
        {
            this.ConditionInfo = ConditionSetHelper.GetCondition(conditionObj)
                                                   .Union(binder.ParseCondition(conditionObj))
                                                   .ToArray();
            return this as TDerive;
        }

        /// <summary>
        /// 設定CTE資訊
        /// </summary>
        /// <param name="cte">CTE內容</param>
        /// <returns></returns>
        public virtual TDerive Cte(string cte)
        {
            this.CteContain = cte;
            return this as TDerive;
        }

        /// <summary>
        /// 淺複製(Shallow Clone)
        /// </summary>
        /// <returns></returns>
        public TDerive Clone()
        {
            return this.MemberwiseClone() as TDerive;
        }
    }
}
