using POCQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.SqlObject
{
    interface IUpdateObject<TDerive> : ICommonObject<TDerive>
    {
        /// <summary>
        /// 設定UPDATE欄位的資料來源來自於哪張Table的哪個欄位；
        /// 使用情境: UPDATE [Table] SET([COLUMNS] = [VALUES]) FROM [Table] INNER JOIN [Other Tabel] ON [CONDITION] WHERE [CONDITION]
        /// </summary>
        /// <param name="sourceTable">資料來源Table</param>
        /// <param name="sourceTableAlias">資料來源Table別名</param>
        /// <param name="sourceCondition">資料來源條件</param>
        /// <param name="columns">
        ///     欄位和資料來源欄位；
        ///     如果有指定資料來源請以':'隔開，字串格式為{Target Column}:{Data Source Column}，Ex:CLASS_NO : FILE_CODE；
        ///     如果沒有指定資料來源，則會自動帶入相同欄位，Ex:CLASS_NO = CLASS_NO : CLASS_NO。
        /// </param>
        /// <returns></returns>
        TDerive ColumnsFrom(string sourceTable, string sourceTableAlias, string sourceCondition, params string[] columns);

        /// <summary>
        /// 達成UPDATE [Table] SET([COLUMNS] = [VALUES]) FROM [Table] INNER JOIN [Other Tabel] ON [CONDITION] WHERE [CONDITION]
        /// </summary>
        /// <param name="sourceTable">資料來源Table</param>
        /// <param name="sourceTableAlias">資料來源Table別名</param>
        /// <param name="sourceCondition">資料來源條件</param>
        /// <returns></returns>
        TDerive InnerJoin(string sourceTable, string sourceTableAlias, string sourceCondition);

    }
}
