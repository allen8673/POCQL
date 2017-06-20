using POCQL.Model;
using POCQL.SqlObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.MSSQL
{
    public sealed class MoveDataObject : BaseMoveDataObject<MoveDataObject>
    {
        internal MoveDataObject()
        {
            this.CteContain = string.Empty;
        }

        internal MoveDataObject(TableSet table)
            : this()
        {
            this.Table = table;
        }

        public override string ToString()
        {
            return $@"DELETE FROM {this.Table.SourceTables.First().TableName} 
                      OUTPUT DELETED.* INTO {this.Table.TableName}
                       WHERE {this.Condition}";
        }
    }
}
