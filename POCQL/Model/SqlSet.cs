using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model
{
    /// <summary>
    /// SQL & SQL參數
    /// </summary>
    public class SqlSet
    {
        public SqlSet(string sql, object parameters) 
        {
            this.Sql = sql;
            this.Parameters = parameters;
        }

        public override string ToString()
        {
            return this.Sql;
        }

        /// <summary>
        /// SQL
        /// </summary>
        public string Sql { get; private set; }

        /// <summary>
        /// SQL參數
        /// </summary>
        public object Parameters { get; private set; }
    }
}
