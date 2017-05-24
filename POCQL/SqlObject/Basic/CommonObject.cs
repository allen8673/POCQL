using POCQL;
using POCQL.Model;
using POCQL.Process.Helper;
using POCQL.ToolExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.SqlObject
{
    public abstract class CommonObject<TDerive> : BaseObject<TDerive>, ICommonObject<TDerive>
        where TDerive : class
    {
        #region Properties
        private string _Condition;
        /// <summary>
        /// SQL條件式(不包含'WHERE'字串)
        /// </summary>
        protected override string Condition
        {
            get
            {
                // *** 解析Condition字串 ***
                if (this._Condition.IsNullOrEmpty())
                    this._Condition = this.ConditionInfo.ConvertToString(this.Table);

                return this._Condition;
            }
            set
            {
                this._Condition = value;
            }
        }
        #endregion

        /// <summary>
        /// 從指定物件(Domain Model)設定Column資訊
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">INSERT/UPDATE欄位的資料，限定為有設定ColumnMapperAttribute的Domain Model</param>
        /// <param name="otherColumns">
        ///     其他指定的欄位與資料來源的對應；
        ///     資料格式為: {Column}:{DataSource};
        ///     Ex: CRT_DT:GETDATE()
        /// </param>
        /// <returns></returns>
        public TDerive Columns<T>(T value, params string[] otherColumns)
            where T : class
        {
            this.ColumnInfos = this.ColumnInfos.Union(Utility.GetColumnValues(value, true).Where(i=>!i.ReadOnly)).ToArray();
            this.Columns(otherColumns);

            return this as TDerive;
        }

        /// <summary>
        /// 從指定欄位對應資料來源字串(格式:{Column}:{DataSource})設定Column資訊
        /// </summary>
        /// <param name="columns">
        ///     指定Column和其對應的資料來源；
        ///     資料格式為: {Column}:{DataSource};
        ///     Ex: CRT_DT:GETDATE()
        /// </param>
        /// <returns></returns>
        public TDerive Columns(params string[] columns)
        {
            List<ColumnSet> columnSets = new List<ColumnSet>();
            foreach (string column in columns)
            {
                // *** 解析欄位對應資料來源字串 ***
                columnSets.Add(ColumnSet.ParseDataSource(column, false, false, false));
            }

            this.ColumnInfos = this.ColumnInfos.Union(columnSets).ToArray();
            return this as TDerive;
        }

        /// <summary>
        /// 從指定欄位對應資料來源字串(格式:{Column}:{DataSource})設定Column資訊
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueBinder">ValueBinder</param>
        /// <returns></returns>
        public TDerive Columns<T>(ValueBinder<T> valueBinder)
        {
            this.ColumnInfos = this.ColumnInfos.Union(valueBinder.ParseColumn()).ToArray();
            return this as TDerive;
        }

        /// <summary>
        /// 從指定Match值設定Column資訊
        /// </summary>
        /// <param name="value">INSERT/UPDATE欄位的資料，限定為有設定ColumnMapperAttribute的Domain Model</param>
        /// <param name="matches">指定的Match值</param>
        /// <returns></returns>
        public TDerive MatcheColumns<T>(T value, params string[] matches)
            where T : class
        {
            this.ColumnInfos = this.ColumnInfos.Concat(Utility.GetMatchColumnValues(value, matches)).ToArray();
            return this as TDerive;
        }

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <returns></returns>
        public override SqlSet Output()
        {
            string sql = this.ToString();
            object parameters = Utility.OutputParameters(this, this.ConditionInfo.ToDictionary());
            return new SqlSet(sql, parameters);
        }

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <param name="otherParam">其他指定參數</param>
        /// <returns></returns>
        public override SqlSet Output(object otherParam)
        {
            string sql = this.ToString();
            object parameters = Utility.OutputParameters(this, this.ConditionInfo.ToDictionary(), otherParam);
            return new SqlSet(sql, parameters);
        }

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <param name="otherParams">其他指定參數</param>
        /// <returns></returns>
        public override IEnumerable<SqlSet> Output(IEnumerable<object> otherParams)
        {
            List<SqlSet> sqlSets = new List<SqlSet>();
            string sql = this.ToString();
            object parameters;

            foreach (object otherParam in otherParams)
            {
                parameters = Utility.OutputParameters(this, this.ConditionInfo.ToDictionary(), otherParam);
                sqlSets.Add(new SqlSet(sql, parameters));
            }

            return sqlSets;
        }

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <typeparam name="T">指定輸出型別</typeparam>
        /// <returns></returns>
        public override T Output<T>()
        {
            return this.Output().ToType<T>();
        }

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="otherParam">其他指定參數</param>
        /// <returns></returns>
        public override T Output<T>(object otherParam)
        {
            return this.Output(otherParam).ToType<T>();
        }

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="otherParams">其他指定參數</param>
        /// <returns></returns>
        public override IEnumerable<T> Output<T>(IEnumerable<object> otherParams)
        {
            return this.Output(otherParams).ToType<T>();
        }
    }
}
