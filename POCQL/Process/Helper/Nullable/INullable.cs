using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Process.Helper
{
    /// <summary>
    /// Nullable設定，用於實現由model外部執行與NullableAttribute相同行為的設定
    /// </summary>
    public interface INullable
    {
        /// <summary>
        /// Nullable Property設定
        /// </summary>
        NullableList NullableProperties { get; set; }
    }
}
