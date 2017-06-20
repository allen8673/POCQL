using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POCQL.Model;
using POCQL.Process.Helper;
using POCQL.ToolExt;

namespace POCQL.SqlObject
{
    public abstract class BaseDeleteObject<TDerive> : BaseObject<TDerive>
        where TDerive : class
    {
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
                    this._Condition = this.ConditionInfo.ConvertToString();

                return this._Condition;
            }
            set
            {
                this._Condition = value;
            }
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
                parameters = Utility.OutputParameters(this.ConditionInfo.ToDictionary(), otherParam);
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

        /// <summary>
        /// 設定CTE資訊
        /// </summary>
        /// <param name="cte">CTE內容</param>
        /// <returns></returns>
        public override TDerive Cte(string cte)
        {
            throw new NotImplementedException("DELETE 不提供CTE功能");
        }

    }
}
