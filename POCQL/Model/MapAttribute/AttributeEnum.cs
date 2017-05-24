
using POCQL.Model.InternalAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model.MapAttribute
{
    class MapAttributeConst
    {
        internal const string IsNull = "{0} IS NULL";
        internal const string IsNotNull = "{0} IS NOT NULL";
    }

    /// <summary>
    /// SQL條件運算子
    /// </summary>
    public enum ConditionOperator
    {
        /// <summary>
        /// Column = Value
        /// </summary>
        [NullDescription(MapAttributeConst.IsNull)]
        [RawDescription("{0} = {1}")]
        [Description("{0} = @{1}")]
        Equal = 0,

        /// <summary>
        /// Column != Value
        /// </summary>
        [NullDescription(MapAttributeConst.IsNotNull)]
        [RawDescription("{0} != {1}")]
        [Description("{0} != @{1}")]
        NotEqual,

        /// <summary>
        /// Column &gt; Value
        /// </summary>
        [NullDescription(MapAttributeConst.IsNotNull)]
        [RawDescription("{0} > {1}")]
        [Description("{0} > @{1}")]
        More,

        /// <summary>
        /// Column &lt; Value
        /// </summary>
        [NullDescription(MapAttributeConst.IsNotNull)]
        [RawDescription("{0} < {1}")]
        [Description("{0} < @{1}")]
        Less,

        /// <summary>
        /// Column IN (val1, val2, ..., valn)
        /// </summary>
        [NullDescription(MapAttributeConst.IsNull)]
        [RawDescription("{0} IN {1}")]
        [Description("{0} IN @{1}")]
        In,

        /// <summary>
        /// Column NOT IN (val1, val2, ..., valn)
        /// </summary>
        [NullDescription(MapAttributeConst.IsNotNull)]
        [RawDescription("{0} NOT IN {1}")]
        [Description("{0} NOT IN @{1}")]
        NotIn,

        /// <summary>
        /// Column &gt;= Value
        /// </summary>
        [NullDescription(MapAttributeConst.IsNull)]
        [RawDescription("{0} >= {1}")]
        [Description("{0} >= @{1}")]
        MoreOrEqual,

        /// <summary>
        /// Column &lt;= Value
        /// </summary>
        [NullDescription(MapAttributeConst.IsNull)]
        [RawDescription("{0} <= {1}")]
        [Description("{0} <= @{1}")]
        LessOrEqual,

        /// <summary>
        /// Column LIKE %Value%
        /// </summary>
        [NullDescription(MapAttributeConst.IsNull)]
        [RawDescription("{0} LIKE '%'+{1}+'%'")]
        [Description("{0} LIKE '%'+@{1}+'%'")]
        Like,

        /// <summary>
        /// Column LIKE Value%
        /// </summary>
        [NullDescription(MapAttributeConst.IsNull)]
        [RawDescription("{0} LIKE {1}+'%'")]
        [Description("{0} LIKE @{1}+'%'")]
        LikeStartWith,

        /// <summary>
        /// Column LIKE %Value
        /// </summary>
        [NullDescription(MapAttributeConst.IsNull)]
        [RawDescription("{0} LIKE '%'+{1}")]
        [Description("{0} LIKE '%'+@{1}")]
        LikeEndWith,

        /// <summary>
        /// Column BETWEEN PrefixValue AND PostfixValue
        /// </summary>
        [NullDescription(MapAttributeConst.IsNull)]
        [RawDescription("{0} BETWEEN {1} AND {2}")]
        [Description("{0} BETWEEN @{1} AND @{2}")]
        Between,

    }

    /// <summary>
    /// Between控制項
    /// </summary>
    public enum BetweenSet
    {
        [Description("Pre_{0}")]
        PrefixValue,
        [Description("Post_{0}")]
        PostfixValue
    }

    /// <summary>
    /// AND, OR運算子
    /// </summary>
    public enum AndOrOperator 
    {
        [Description(" AND ")]
        AND,

        [Description(" OR ")]
        OR
    }

    /// <summary>
    /// Aggregate Function
    /// </summary>
    public enum AggregateFunction
    {
        [Description("AVG({#COLUMN#})")]
        AVG,
        [Description("COUNT({#COLUMN#})")]
        COUNT,
        [Description("FIRST({#COLUMN#})")]
        FIRST,
        [Description("LAST({#COLUMN#})")]
        LAST,
        [Description("MAX({#COLUMN#})")]
        MAX,
        [Description("MIN({#COLUMN#})")]
        MIN,
        [Description("SUM({#COLUMN#})")]
        SUM
    }

    /// <summary>
    /// 映射型態
    /// </summary>
    public enum MapType 
    {
        /// <summary>
        /// 欄位對物件屬性的映射
        /// </summary>
        Column2Property = 0,
        /// <summary>
        /// 欄位對欄位的映射
        /// </summary>
        Column2Column
    }

    /// <summary>
    /// 【ColumnMapperAttribute】專屬映射型態
    /// </summary>
    public enum ColumnType
    {
        /// <summary>
        /// 映射欄位
        /// </summary>
        Column = 0,
        /// <summary>
        /// 映射資料來源
        /// </summary>
        DataSource,
    }

}
