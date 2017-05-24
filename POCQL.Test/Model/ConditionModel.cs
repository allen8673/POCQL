using POCQL.Model.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Test.Example.Model
{
    [EntityMapper("{#TABLE#}")]
    public class ConditionModel
    {
        [Condition("Column_1")]
        public string Property_1 { get; set; }

        // *** 如果條件運算子為BETWEEN，則需要在型別Model中追加設定 ***
        [Condition("Column_2",  ConditionOperator.Between)]
        public IntervalVal Property_2 { get; set; }

        // *** 如果條件運算子為IN/NOT IN，可以直接指定Property型態為Array ***
        [Condition("Column_3", "Table_1", ConditionOperator.In)]
        public string[] Property_3 { get; set; }

        [Condition("Column_1", "Table_2")]
        public DateTime? Property_4 { get; set; }

        [Condition("Column_2", "Table_1")]
        public string Property_5 { get; set; }

        [Condition("Column_3", "Table_2", ConditionOperator.Like)]
        public string Property_6 { get; set; }

        [Condition("Column_3", "{#TABLE#}", ConditionOperator.Like)]
        public string Property_7 { get; set; }
    }
}
