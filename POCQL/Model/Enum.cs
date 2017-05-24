using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model
{
    /// <summary>
    /// 排序方式
    /// </summary>
    public enum SortKind
    {
        DESC,
        ASC
    }
    
    /// <summary>
    /// Join類型
    /// </summary>
    public enum JoinType 
    {
        [Description("INNER JOIN")]
        InnerJoin,
        [Description("LEFT JOIN")]
        LeftJoin
    }

    /// <summary>
    /// Aggregate Function
    /// </summary>
    public enum Aggregate
    {
        [Description("AVG")]
        AVG,
        [Description("COUNT")]
        COUNT,
        [Description("FIRST")]
        FIRST,
        [Description("LAST")]
        LAST,
        [Description("MAX")]
        MAX,
        [Description("MIN")]
        MIN,
        [Description("SUM")]
        SUM
    }

    static class Enum_Ext 
    {
        /// <summary>
        /// 將字串轉成ENUM
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static T ToEnum<T>(this string value)
            where T : struct, IConvertible
        {
            T result;
            if (!Enum.TryParse<T>(value, true, out result))
                throw new Exception("序列無相對應項目");

            return result;
        }
    }
}
