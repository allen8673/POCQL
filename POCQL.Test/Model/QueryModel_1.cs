using POCQL.Model.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Test.Example.Model
{
    [EntityMapper]
    public class QueryModel_1
    {
        [ColumnMapper("Column_1")]
        public string Property_1 { get; set; }

        [ColumnMapper("Column_2", "Table2")]
        public string Property_2 { get; set; }

        [Nullable]
        [ColumnMapper("Column_3")]
        public string Property_3 { get; set; }

        [ColumnMapper("Column_4 : DataSource")]
        public DateTime? Property_4 { get; set; }

        [ColumnMapper("Column_5")]
        public string Property_5 { get; set; }

        [ColumnMapper("Column_6")]
        public string Property_6 { get; set; }

        [ColumnMapper("Column_7")]
        public string Property_7 { get; set; }

        [Customize("MOST")]
        [ColumnMapper("Customization_Column1")]
        public string CustomizationProp1 { get; set; }

        [Customize("MOST", "CPA")]
        [ColumnMapper("Customization_Column2")]
        public string CustomizationProp2 { get; set; }

        [Customize("CPA")]
        [ColumnMapper("Customization_Column3")]
        public string CustomizationProp3 { get; set; }

        public QueryModel_2 SubType { get; set; }
    }
}
