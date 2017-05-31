using POCQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.SqlObject
{
    interface IConditionObject<TDerive>
    {
        /// <summary>
        /// 設定WHERE資訊
        /// </summary>
        /// <param name="condition">WHERE條件式</param>
        /// <returns></returns>
        TDerive Where(string condition);

        /// <summary>
        /// 設定WHERE資訊
        /// </summary>
        /// <param name="conditionObj">經由ColumnMapperAttribute設定的條件式物件</param>
        /// <returns></returns>
        TDerive Where<T>(T conditionObj) where T : class;

        /// <summary>
        /// 設定WHERE資訊
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionObj">經由ColumnMapperAttribute設定的條件式物件</param>
        /// <param name="paramTemplate">條件物件參數Template，請以XXX{#PROP#}XX給</param>
        /// <returns></returns>
        TDerive Where<T>(T conditionObj, string paramTemplate) where T : class;

        /// <summary>
        /// 設定WHERE資訊
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionObj">經由ColumnMapperAttribute設定的條件式物件</param>
        /// <param name="binder">條件物件與SQL條件binder</param>
        /// <returns></returns>
        TDerive Where<T>(T conditionObj, ConditionBinder binder) where T : class;

        /// <summary>
        /// 設定WHERE資訊
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionObj">經由ColumnMapperAttribute設定的條件式物件</param>
        /// <param name="binder">條件物件與SQL條件binder</param>
        /// <returns></returns>
        TDerive Where<T>(T conditionObj, ConditionBinder<T> binder) where T : class;
    }
}
