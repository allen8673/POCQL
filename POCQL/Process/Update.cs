using POCQL.Model;
using POCQL.SqlObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POCQL.MSSQL;
using System.Diagnostics;

namespace POCQL
{
    public static class Update
    {
        /// <summary>
        /// 指定UPDATE的Table及欄位資料，並且產生UpdateObject
        /// </summary>
        /// <param name="table">欲UPDATE的Table</param>
        /// <param name="updateValue">欲UPDATE的欄位資料</param>
        /// <returns></returns>
        public static UpdateObject Columns<T>(string table, T updateValue)
            where T : class
        {
            IEnumerable<ColumnSet> columnInfos = Utility.GetColumnValues(updateValue, true);
            TableSet tableSet = new TableSet(table);

            return new UpdateObject(columnInfos, tableSet);
        }

        /// <summary>
        /// 指定UPDATE的Table及欄位資料，並且產生UpdateObject
        /// </summary>
        /// <param name="table">欲UPDATE的Table</param>
        /// <param name="alias">Table別名</param>
        /// <param name="updateValue">欲UPDATE的欄位資料</param>
        /// <returns></returns>
        public static UpdateObject Columns<T>(string table, string alias, T updateValue)
            where T : class
        {
            IEnumerable<ColumnSet> columnInfos = Utility.GetColumnValues(updateValue, true);
            TableSet tableSet = new TableSet(table, alias);

            return new UpdateObject(columnInfos, tableSet);
        }

        /// <summary>
        /// 指定UPDATE的Table，並且產生UpdateObject
        /// </summary>
        /// <param name="table">欲UPDATE的Table</param>
        /// <returns></returns>
        public static UpdateObject Table(string table)
        {
            return new UpdateObject(new TableSet(table));
        }

        /// <summary>
        /// 指定UPDATE的Table，並且產生UpdateObject
        /// </summary>
        /// <param name="table">欲UPDATE的Table</param>
        /// <param name="alias">Table別名</param>
        /// <returns></returns>
        public static UpdateObject Table(string table, string alias)
        {
            return new UpdateObject(new TableSet(table, alias));
        }

    }
}
