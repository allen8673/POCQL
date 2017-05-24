using POCQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.SqlObject
{
    public abstract class BaseInsertObject<TDerive> : CommonObject<TDerive>, IInsertObject<TDerive>
        where TDerive : class
    {
        /// <summary>
        /// 設定INSER欄位的資料來源來自於哪張Table的哪個欄位
        /// 使用情境: INSERT INTO [Table] VALUES([COLUMNS]) SELECT [COLUMNS] FROM [Table] WHERE [CONDITION]
        /// </summary>
        /// <param name="sourceTable">資料來源Table，可以{Tabel} {TabelAlias}格式給予Table別名</param>
        /// <param name="columns">
        ///     欄位和資料來源欄位
        ///     如果有指定資料來源請以':'隔開，字串格式為{Target Column}:{Data Source Column}，Ex:CLASS_NO : FILE_CODE
        ///     如果沒有指定資料來源，則會自動帶入相同欄位，Ex:CLASS_NO = CLASS_NO : CLASS_NO
        /// </param>
        /// <returns></returns>
        public TDerive ColumnsFrom(string sourceTable, params string[] columns)
        {
            string[] table = sourceTable.Split(new char[0]);

            // *** 設定來源Table ***
            this.Table.SetSourceTable(table[0], table.Count() < 2 ? null : table[1]);
            
            List<ColumnSet> columnSets = new List<ColumnSet>();
            foreach (string column in columns)
            {
                // *** 新增資料來源 ***
                columnSets.Add(ColumnSet.ParseDataSource(column, this.Table.TableName, table[0], false, false, false));
            }

            this.ColumnInfos = this.ColumnInfos.Concat(columnSets).ToArray();
            return this as TDerive;
        }
    }
}
