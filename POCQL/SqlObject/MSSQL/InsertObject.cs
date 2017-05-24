using POCQL.Model;
using POCQL.SqlObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace POCQL.MSSQL
{
    public sealed class InsertObject : BaseInsertObject<InsertObject>, IInsertObject<InsertObject>
    {
        internal InsertObject()
        {
            this.CteContain = string.Empty;
        }

        internal InsertObject(TableSet table)
            : this()
        {
            this.Table = table;
        }

        /// <summary>
        /// 依'this.Table.HasSourceTables'取得對應的ToString方法
        /// </summary>
        private Dictionary<bool, Func<string>> ToStringHash
        {
            get
            {
                return new Dictionary<bool, Func<string>>
                {
                    {
                        true, new Func<string> (()=>
                        {
                            // *** 先過濾掉ReadOnly的Property (ColumnSet) ***
                            IEnumerable<ColumnSet> columnInfos = this.ColumnInfos.Where(i => !i.ReadOnly);
                            // *** 處理Insert欄位的部分 ***
                            string columns = string.Join(",", columnInfos.Select(i => i.ColumnName));
                            var sourceTable = this.Table.SourceTables.First();
                            IEnumerable<ColumnSet> columnSets = columnInfos.Select(i =>
                            {
                                return i.DataSource == null ?
                                       ColumnSet.ParseDataSource($"{i.ColumnName}:@{i.ColumnName}", i.IsPrimaryKey, i.ReadOnly, i.Aggregate) :
                                       ColumnSet.ParseDataSource($"{i.ColumnName}:{i.DataSource.SourceValue}", i.IsPrimaryKey, i.ReadOnly, i.Aggregate);
                            });
                            SelectObject select = Select.Columns(columnSets.ToArray(), false)
                                                        .From(sourceTable.TableName, sourceTable.Alias)
                                                        .Where(this.Condition);
                            return this.CteContain + Environment.NewLine
                                 + $"INSERT INTO {this.Table.TableName}" + Environment.NewLine
                                 + $"       {(string.IsNullOrEmpty(columns) ? string.Empty : $"({columns})")}" + Environment.NewLine
                                 + select;
                        })
                    },
                    {
                        false, new Func<string> (()=>
                        {
                            // *** 先過濾掉ReadOnly的Property (ColumnSet) ***
                            IEnumerable<ColumnSet> columnInfos = this.ColumnInfos.Where(i => !i.ReadOnly);
                            // *** 處理Insert欄位的部分 ***
                            string columns = string.Join(",", columnInfos.Select(i => i.ColumnName));
                            string values = string.Join(",", columnInfos.Select(i =>
                            {
                                return i.DataSource == null ?
                                       $"@{i.ColumnName}" :
                                       i.DataSource.SourceValue;
                            }));

                            return this.CteContain + Environment.NewLine
                                 + $"INSERT INTO {this.Table.TableName}" + Environment.NewLine
                                 + $"       ({columns})" + Environment.NewLine
                                 + $"VALUES ({values})";
                        })
                    }
                };
            }
        }

        public override string ToString()
        {
            string sql = this.ToStringHash[this.Table.HasSourceTables]();
            return Regex.Replace(sql, @"[ \t]+", " ");
        }

    }
}

