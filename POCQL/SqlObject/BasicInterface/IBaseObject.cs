using POCQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.SqlObject
{
    interface IBaseObject<TDerive> : ICondition<TDerive>
    {
        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <returns></returns>
        SqlSet Output();

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <param name="otherParam">其他指定參數</param>
        /// <returns></returns>
        SqlSet Output(object otherParam);

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <param name="otherParams">其他指定參數</param>
        /// <returns></returns>
        IEnumerable<SqlSet> Output(IEnumerable<object> otherParams);

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <typeparam name="T">指定輸出型別</typeparam>
        /// <returns></returns>
        T Output<T>();

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="otherParam">其他指定參數</param>
        /// <returns></returns>
        T Output<T>(object otherParam);

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="otherParams">其他指定參數</param>
        /// <returns></returns>
        IEnumerable<T> Output<T>(IEnumerable<object> otherParams);

        /// <summary>
        /// 設定CTE資訊
        /// </summary>
        /// <param name="cte">CTE內容</param>
        /// <returns></returns>
        TDerive Cte(string cte);
    }
}
