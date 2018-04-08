using POCQL.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POCQL.Model
{
    /// <summary>
    /// 主要Table資訊
    /// </summary>
    public class TableSet<T> : STMOperator<T>
        where T : class
    {
        /// <summary>
        /// Table名
        /// </summary>
        public string TableName { get; protected set; }

        /// <summary>
        /// 別名
        /// </summary>
        public string Alias { get; protected set; }

        /// <summary>
        /// 次要(INNER/LEFT JOIN or Others...)Table
        /// </summary>
        public SourceTableSet[] SourceTables { get; protected set; }

        /// <summary>
        /// 是否有次要Table
        /// </summary>
        public bool HasSourceTables { get { return this.SourceTables.Count() > 0; } }

        /// <summary>
        /// 次要Table資訊
        /// </summary>
        public class SourceTableSet : TableSet<SourceTableSet>
        {
            public SourceTableSet(string name)
            {
                this.TableName = name;
            }

            public SourceTableSet(string name, string alias)
                : this(name)
            {
                this.Alias = alias;
            }

            public SourceTableSet(string name, string alias, bool lockTable, JoinType join, string condition)
                : this(name, alias)
            {
                this.Join = join;
                this.JoinCondition = condition;
                this.LockTable = lockTable;
            }

            /// <summary>
            /// INNER/LEFT JOIN條件式
            /// </summary>
            public string JoinCondition { get; private set; }

            /// <summary>
            /// JOIN類型
            /// </summary>
            public JoinType Join { get; private set; }

            /// <summary>
            /// If Table (NOLOCK)
            /// </summary>
            public bool LockTable { get; set; }
        }
    }

    /// <summary>
    /// Table資訊
    /// </summary>
    public class TableSet : TableSet<TableSet>
    {
        protected TableSet()
        {
            this.SourceTables = new SourceTableSet[] { };
        }

        public TableSet(string name)
            : this()
        {
            this.TableName = name;
        }

        public TableSet(string name, string alias)
            : this(name)
        {
            this.Alias = alias;
        }

        /// <summary>
        /// 設定Table資訊
        /// </summary>
        /// <param name="name">Table Name</param>
        public void SetTable(string name)
        {
            this.TableName = name;
        }

        /// <summary>
        /// 設定Table資訊
        /// </summary>
        /// <param name="name">Table Name</param>
        /// <param name="alias">Table別名</param>
        public void SetTable(string name, string alias)
        {
            this.TableName = name;
            this.Alias = alias;
        }

        /// <summary>
        /// 設定次要Table資訊
        /// </summary>
        /// <param name="name">次要Table Name</param>
        /// <param name="alias"></param>
        /// <param name="join"></param>
        /// <param name="condition"></param>
        public void SetSourceTable(string name, string alias, bool lockTable, JoinType join, string condition)
        {
            this.SourceTables += new SourceTableSet(name, alias, lockTable, join, condition);
        }

        /// <summary>
        /// 設定次要Table資訊
        /// </summary>
        /// <param name="name"></param>
        /// <param name="alias"></param>
        public void SetSourceTable(string name, string alias)
        {
            this.SourceTables += new SourceTableSet(name, alias);
        }

        /// <summary>
        /// Table & 對應別名
        /// </summary>
        public Dictionary<string, string> TableAliasMap
        {
            get
            {
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { this.TableName, this.Alias}
                };

                this.SourceTables
                    .ToList()
                    .ForEach(i => dict.Add(i.TableName, i.Alias));

                return dict.Where(i => !i.Key.IsNullOrEmpty()).ToDictionary(k => k.Key, v => v.Value);
            }
        }
    }
}
