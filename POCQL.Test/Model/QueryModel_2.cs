using POCQL.Model.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Test.Example.Model
{
    [EntityMapper("Table_1")]
    public class QueryModel_2
    {
        //文別
        [ColumnMapper("Column_1")]
        public string Property_1 { get; set; }

        //來文機關
        [ColumnMapper("Column_2")]
        public string Property_2 { get; set; }

        //本別
        [ColumnMapper("Column_3")]
        public string Property_3 { get; set; }
    }
}
