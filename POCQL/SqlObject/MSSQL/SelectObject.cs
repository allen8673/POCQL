using POCQL.Model;
using POCQL.SqlObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using POCQL.Extension;

namespace POCQL.MSSQL
{
    public sealed class SelectObject : BaseSelectObject<SelectObject>
    {
        internal SelectObject(params ColumnSet[] columns)
        {
            this.ColumnInfos = columns;
            this.IgnoreDataSource = true;
        }

        internal SelectObject(IEnumerable<ColumnSet> columns)
        {
            this.ColumnInfos = columns.ToArray();
            this.IgnoreDataSource = true;
        }

        internal SelectObject(IEnumerable<ColumnSet> columns, bool ignoreDataSource)
            : this(columns)
        {
            this.IgnoreDataSource = ignoreDataSource;
        }

        internal SelectObject(IEnumerable<ColumnSet> columns, bool distinct, bool ignoreDataSource)
            : this(columns, ignoreDataSource)
        {
            this.IsDistinct = distinct;
        }

        internal SelectObject(IEnumerable<ColumnSet> columns, int top, bool ignoreDataSource)
            : this(columns, ignoreDataSource)
        {
            this.TopCount = top;
        }

        public override string Sql { get { return this.ToString(); } }
        public override string CountSql { get { return this.ToCountSql(); } }
        public override string ExistSql { get { return this.ToExistsSql(); } }

        /// <summary>
        /// 是否忽略DataSource；
        /// 如果不忽略資料來源，在SELECT時，
        /// 物件Property Value都會從Attribute的設定來。
        /// </summary>
        private bool IgnoreDataSource { get; set; }

        public override string ToString()
        {
            return this.CteContain +
                   Environment.NewLine +
                   this.ToSql();
        }

        /// <summary>
        /// 將SelectObject輸出成SQL
        /// </summary>
        /// <returns></returns>
        private string ToSql()
        {
            // *** 先取得一般的SQL ***
            string baseSql = this.ToCommonString();

            // *** 先記錄Table資訊 ***
            Dictionary<string, string> table2Alias = this.Table.TableAliasMap;

            // *** 處理ORDER BY的部分 ***
            string orderBy = string.Empty;
            if (this.OrderByColumn.Count() != 0)
            {
                List<string> orderByItems = new List<string>();
                string table;
                ColumnSet columnset;

                foreach (OrderBySetting odrSetting in this.OrderByColumn)
                {
                    // *** 先找出有相同Column Name的ColumnSet ***
                    columnset = this.ColumnInfos.FirstOrDefault(i => i.ColumnName == odrSetting.OrderByColumn);
                    // *** 取得Column對應的Table名稱 ***
                    table = columnset != null ? columnset.TableDetailProcess(table2Alias) : string.Empty;
                    // *** 將結果(ORDER BY項目)加入至List中 ***
                    orderByItems.Add($"{new string[] { table, odrSetting.OrderByColumn }.StringJoin(".")} {odrSetting.Sort}");
                }

                orderBy = $"ORDER BY {orderByItems.StringJoin(",")}";
            }

            // *** 處理Pagging的部分 ***
            string pagging = string.Empty;
            if (!string.IsNullOrEmpty(orderBy) && this.Page > 0 && this.PageRow > 0)
            {
                pagging = $"OFFSET {(this.Page - 1) * this.PageRow} ROWS FETCH FIRST {this.PageRow} ROWS ONLY";
            }

            // *** 整合SQL，包含CTE ***
            string sql = $@"{baseSql}
                            {orderBy} 
                            {pagging}";

            return Regex.Replace(sql, @"[ \t]+", " ");
        }

        /// <summary>
        /// 將SelectObject輸出成取得COUNT(*) SQL
        /// </summary>
        /// <returns></returns>
        private string ToCountSql()
        {
            // *** 先取得一般的SQL ***
            string baseSql = this.ToCommonString(),

            // *** 將baseSql放進CTE中 ***
            cte = this.CteContain.IsNullOrEmpty() ?
                         $";WITH SQL_CET AS({baseSql})" :
                         $"{this.CteContain},SQL_CET AS({baseSql})",

            // *** 整合SQL，包含CTE ***
            sql = cte +
                  Environment.NewLine +
                  "SELECT COUNT(*) [result] FROM SQL_CET";

            return Regex.Replace(sql, @"[ \t]+", " ");
        }

        /// <summary>
        /// 將SelectObject輸出成SELECT CASE WHEN EXISTS ...
        /// </summary>
        /// <returns></returns>
        private string ToExistsSql()
        {
            return this.CteContain +
                   Environment.NewLine +
                   $@"SELECT CASE WHEN EXISTS({this.ToSql()}) 
                                  THEN 1 
                                  ELSE 0 END";

        }

        /// <summary>
        /// 將SelectObject轉成SQL字串；
        /// 沒有CTE和Paging
        /// </summary>
        /// <returns></returns>
        private string ToCommonString()
        {
            // *** 先記錄Table資訊 ***
            Dictionary<string, string> table2Alias = this.Table.TableAliasMap;

            // *** 處理Column的部分 ***
            IEnumerable<string> columns = this.ParseColumns(this.ColumnInfos, table2Alias, true);

            // *** 處理GROUP BY的部分 ***
            string groupBy = string.Empty;

            if (this.GroupByItems.Count() > 0 ||
                // *** Aggregate欄位和非Aggregate欄位同時並存才要做Group By，否則不用 ***
                (this.ColumnInfos.Any(i => i.Aggregate) && this.ColumnInfos.Any(i => !i.Aggregate)))
            {
                // *** 這是用來做欄位塞選的；
                //     不能保證ColumnSet.ColumnName都是理想的欄位名稱，
                //     所以要針對ColumnSet.ColumnName做篩選 ***
                Regex rgx = new Regex(@"(\w+[.])?(\w+)");

                // *** 宣告共用條件找出GROUP BY對象的條件 ***
                Func<ColumnSet, bool> commonFilter = (columnSet) => !columnSet.Aggregate && !columnSet.IsNullColumn;
                // *** 宣告利用GroupByItem從ColumnInfos找出GROUP BY對象的條件 ***
                Func<ColumnSet, bool> groupByItemFilter = (columnSet) => this.GroupByItems.Any(j => columnSet.ColumnName == j || columnSet.Property == j);
                // *** 宣告利用ColumnInfos本身找出GROUP BY對象的條件 ***
                Func<ColumnSet, bool> columnInfoFilter = (columnSet) => this.ColumnInfos.Any(c => c.Aggregate) && rgx.Matches(columnSet.ColumnName).Count == 1;

                // *** 如果欲查詢欄位中有Aggregate運算，其他欄位就需要做Group By；
                //     所以在此情況下需要將'非Aggregate運算'列為篩選條件 ***
                // *** 反之，只需要對使用者給予的GroupByItems與ColumnInfos做比對即可 ***
                IEnumerable<ColumnSet> columnsets = this.ColumnInfos.Where(i => commonFilter(i) && (groupByItemFilter(i) || columnInfoFilter(i)));
                // *** 如果物件沒有對應的prop，就直接拿GroupByItems做Group By ***
                IEnumerable<string> groupColumns = columnsets.Count() > 0 ? this.ParseColumns(columnsets, table2Alias, false).Concat(this.GroupByItems)
                                                                          : this.GroupByItems;
                groupBy = $"GROUP BY {string.Join(",", groupColumns)}";
            }

            // *** 整合SQL ***
            return $@"SELECT {(this.IsDistinct ? "DISTINCT" : string.Empty)} 
                             {(this.TopCount > 0 ? $"TOP {this.TopCount}" : string.Empty)} 
                             {(columns.Count() > 0 ? string.Join(",", columns) : "*")} 
                        FROM {$"{this.Table.TableName} {this.Table.Alias ?? string.Empty}"} {(!this.LockTable ? "WITH (NOLOCK)" : string.Empty)}
                             {this.GetJoin()}
                        {(this.Condition.IsNullOrEmpty() ? string.Empty : $"WHERE {this.Condition}")} 
                        {groupBy}";
        }

        /// <summary>
        /// 解析Column資訊，取得要查詢的Columns
        /// </summary>
        /// <param name="columnInfos">欲解析的ColumnSet陣列</param>
        /// <param name="table2Alias">Table對應別名資訊</param>
        /// <param name="mapProperty">是否Map至Property</param>
        /// <returns></returns>
        private IEnumerable<string> ParseColumns(IEnumerable<ColumnSet> columnInfos, Dictionary<string, string> table2Alias, bool mapProperty)
        {
            List<string> columns = new List<string>();

            foreach (var columnInfo in columnInfos)
            {
                columns.Add(columnInfo.ToString(table2Alias, this.IgnoreDataSource, mapProperty));
            }

            return columns;
        }

        /// <summary>
        /// 取得INNER JOIN SQL 字串
        /// </summary>
        /// <returns></returns>
        private string GetJoin()
        {
            string join = string.Empty;
            foreach (var item in this.Table.SourceTables)
            {
                join += Environment.NewLine
                        + $"{item.Join.GetDescription()} {item.TableName} {item.Alias ?? string.Empty} {(!item.LockTable ? "WITH (NOLOCK)" : string.Empty)} ON {item.JoinCondition}";
            }

            return join;
        }

        /// <summary>
        /// 將Table Parameter換掉
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string TableParametersSetting(string sql)
        {
            foreach (var tabelParameterMap in this.TabelParameterMaps)
            {
                sql = sql.Replace(tabelParameterMap.Key, tabelParameterMap.Value);
            }

            return sql;
        }

    }
}
