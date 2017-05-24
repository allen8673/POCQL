using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model
{
    public class OrderBySetting : STMOperator<OrderBySetting>
    {
        internal OrderBySetting(string column, SortKind sort)
        {
            this.OrderByColumn = column;
            this.Sort = sort;
        }

        /// <summary>
        /// Order By目標欄位名稱
        /// </summary>
        internal string OrderByColumn { get; private set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        internal SortKind Sort { get; private set; }
    }
}
