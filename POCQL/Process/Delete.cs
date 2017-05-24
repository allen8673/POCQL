using POCQL.Model;
using POCQL.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL
{
    public class Delete
    {
        /// <summary>
        /// 指定DELETE的Table，並且產生DeleteObject
        /// </summary>
        /// <param name="table">欲DELETE的Table</param>
        /// <returns></returns>
        public static DeleteObject Table(string table)
        {
            return new DeleteObject(new TableSet(table));
        }
    }
}
