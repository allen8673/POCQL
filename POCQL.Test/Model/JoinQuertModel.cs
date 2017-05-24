using POCQL.Model.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Test.Example.Model
{
    /// <summary>
    /// 查詢欄位跨Table的範例Model
    /// </summary>
    //[EntityMapper("Table_1")]    
    [EntityMapper("{#TABLE#}")]
    public class JoinQuertModel
    {
        [ColumnMapper("Column_1")]
        public string Property_1 { get; set; }

        [ColumnMapper("Column_2")]
        public string Property_2 { get; set; }

        [ColumnMapper("Column_3")]
        public string Property_3 { get; set; }

        [ColumnMapper("Column_4")]
        public DateTime? Property_4 { get; set; }

        [ColumnMapper("Column_1", "Table_2")]
        public string Property_5 { get; set; }

        [ColumnMapper("Column_2", "Table_2")]
        public string Property_6 { get; set; }

        [ColumnMapper("Column_1", "Table_3")]
        public string Property_7 { get; set; }
    }
}
