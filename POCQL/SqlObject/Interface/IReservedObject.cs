using POCQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.SqlObject
{
    interface IReservedObject<TDerive>
    {
        /// <summary>
        /// Select Top ...
        /// </summary>
        /// <param name="top">Top Number</param>
        /// <returns></returns>
        TDerive Top(int top);

        /// <summary>
        /// 設定ORDER BY
        /// </summary>
        /// <param name="orderBy">要Order By的欄位或Property</param>
        /// <param name="defaultOrder">如果orderBy為空時，預設排序</param>
        /// <param name="sort">排序方式:ASC,DESC</param>
        /// <returns></returns>
        TDerive OrderBy(string orderBy, string defaultOrder, string sort);

        /// <summary>
        /// 設定ORDER BY
        /// </summary>
        /// <param name="orderBy">要Order By的欄位或Property</param>
        /// <param name="sort">排序方式:ASC,DESC</param>
        /// <returns></returns>
        TDerive OrderBy(string orderBy, string sort);

        /// <summary>
        /// 設定ORDER BY
        /// </summary>
        /// <param name="orderBy">要Order By的欄位或Property</param>
        /// <returns></returns>
        TDerive OrderBy(string orderBy);

        /// <summary>
        /// 設定GROUP BY
        /// </summary>
        /// <param name="groupBy">要Group By的欄位或Property</param>
        /// <returns></returns>
        TDerive GroupBy(params string[] groupBy);

        /// <summary>
        /// 設定分頁
        /// </summary>
        /// <param name="pageRow">每頁資料量</param>
        /// <param name="page">第幾頁</param>
        /// <returns></returns>
        TDerive Paging(int pageRow, int page);
    }
}
