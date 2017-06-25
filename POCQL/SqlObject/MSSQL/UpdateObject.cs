using POCQL.Model;
using POCQL.SqlObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using POCQL.Extension;

namespace POCQL.MSSQL
{
    public sealed class UpdateObject : BaseUpdateObject<UpdateObject>
    {
        internal UpdateObject()
        {
            this.CteContain = string.Empty;
        }

        internal UpdateObject(TableSet table)
            : this()
        {
            this.Table = table;
        }

        internal UpdateObject(IEnumerable<ColumnSet> columns, TableSet table)
            : this(table)
        {
            this.ColumnInfos = columns.ToArray();
        }

        public override string ToString()
        {
            // *** 先記錄Table資訊 ***
            Dictionary<string, string> tableAliases = new Dictionary<string, string>()
            {
                { this.Table.TableName, this.Table.Alias}
            };

            if (this.Table.SourceTables.Count() != 0)
            {
                var sourceTable = this.Table.SourceTables.First();
                tableAliases.Add(sourceTable.TableName, sourceTable.Alias);
            }
            

            // *** 處理Update欄位的部分 ***
            string update = string.Join(",", this.ParseUpdateItems(tableAliases));

            // *** 整合SQL ***
            string sql = new Dictionary<bool, string>
            {
                { true, $@"{this.CteContain}
                            UPDATE {(!this.Table.Alias.IsNullOrEmpty() ? this.Table.Alias : this.Table.TableName)}
                               SET {update}
                              FROM {$"[{ this.Table.TableName}] {this.Table.Alias ?? string.Empty }"}
                              {this.GetSource()}
                             WHERE {this.Condition}"},
                { false, $@"{this.CteContain}
                           UPDATE {$"[{ this.Table.TableName}] {this.Table.Alias ?? string.Empty }"}
                              SET {update}
                            WHERE {this.Condition}"}
            }[this.Table.HasSourceTables];

            return Regex.Replace(sql, @"[ \t]+", " ");
        }

        /// <summary>
        /// 解析Update項目
        /// </summary>
        /// <param name="table2Alias">Table與別名的對應</param>
        /// <returns></returns>
        private List<string> ParseUpdateItems(Dictionary<string, string> table2Alias)
        {
            List<string> updateItems = new List<string>();
            string column, table, sourceValue, sourceTable;

            foreach (var columnInfo in this.ColumnInfos)
            {
                // *** 如果是Primary Key，Update就不會去異動資料 ***
                // *** 如果是ReadOnly，直接忽略不處理 ***
                if (columnInfo.IsPrimaryKey || columnInfo.ReadOnly) continue;

                table = columnInfo.TableName ?? string.Empty;
                table = table2Alias.ContainsKey(table) ? table2Alias[table] : table;
                column = new string[] { table, columnInfo.ColumnName }.StringJoin(".");

                if (columnInfo.DataSource == null)
                {
                    updateItems.Add($"{column} = @{columnInfo.ColumnName}");
                }                    
                else 
                {
                    sourceTable = columnInfo.DataSource.SourceTable ?? string.Empty;
                    sourceTable = table2Alias.ContainsKey(sourceTable) ? table2Alias[sourceTable] : sourceTable;
                    sourceValue = new string[] { sourceTable, columnInfo.DataSource.SourceValue }.StringJoin(".");
                    updateItems.Add($"{column} = {sourceValue}");
                }

            }

            return updateItems;
        }

        /// <summary>
        /// 取得來源Table相關SQL字串
        /// </summary>
        /// <returns></returns>
        private string GetSource() 
        {
            if (this.Table.SourceTables.Count() == 0)
                return string.Empty;

            var sourceTable = this.Table.SourceTables.First();

            return $"INNER JOIN {$"[{sourceTable.TableName}] {sourceTable.Alias ?? string.Empty}"} ON {sourceTable.JoinCondition}";
        }
    }
}
