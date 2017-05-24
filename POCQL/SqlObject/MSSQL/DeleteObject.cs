using POCQL.SqlObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POCQL.Model;

namespace POCQL.MSSQL
{
    public sealed class DeleteObject : BaseDeleteObject<DeleteObject>, IDeleteObject<DeleteObject>
    {
        internal DeleteObject()
        {
            this.CteContain = string.Empty;
        }

        internal DeleteObject(TableSet table)
            : this()
        {
            this.Table = table;
        }

        public override string ToString()
        {
            return $@"DELETE FROM {this.Table.TableName} WHERE {this.Condition}";
        }
    }
}
