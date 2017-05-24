using POCQL.Model;
using POCQL.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL
{
    public class MoveData
    {
        public static MoveDataObject From(string table)
        {
            TableSet tableSet = new TableSet(string.Empty);
            tableSet.SetSourceTable(table, string.Empty);

            return new MoveDataObject(tableSet);
        }
    }
}
