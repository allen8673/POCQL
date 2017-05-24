using POCQL.Model;
using POCQL.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL
{
    /// <summary>
    /// 
    /// </summary>
    public static class Insert
    {
        /// <summary>
        /// 指定INSERT的Table，並且產生InsertObject
        /// </summary>
        /// <param name="table">欲INSERT的Table</param>
        /// <returns></returns>
        public static InsertObject Table(string table)
        {
            return new InsertObject(new TableSet(table));
        }
    }
}
