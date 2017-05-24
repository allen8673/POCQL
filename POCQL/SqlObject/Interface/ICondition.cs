using POCQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.SqlObject
{
    interface ICondition<TDerive>
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
        /// <param name="otherCondition">其他WHERE條件式物件</param>
        /// <returns></returns>
        TDerive Where<T>(T conditionObj, params ConditionSet[] otherCondition)
            where T:class;
    }
}
