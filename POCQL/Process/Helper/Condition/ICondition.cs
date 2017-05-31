using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Process.Helper
{
    /// <summary>
    /// Condition設定，用於實現由model外部執行與ColumnMapperAttribute相同行為的設定
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// Condition物件Property和Column的連結設定
        /// </summary>
        ConditionMapperSetting ConditionSetting { get; set; }
    }
}
